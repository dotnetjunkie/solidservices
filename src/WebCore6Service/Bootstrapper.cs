using BusinessLayer;
using SimpleInjector;
using System.Diagnostics;
using System.Security.Principal;

namespace WebCoreService;

public static class Bootstrapper
{
    public static IEnumerable<Type> GetKnownCommandTypes() => BusinessLayerBootstrapper.GetCommandTypes();

    public static IEnumerable<QueryInfo> GetKnownQueryTypes() => BusinessLayerBootstrapper.GetQueryTypes();

    public static Container Bootstrap(Container container)
    {
        BusinessLayerBootstrapper.Bootstrap(container);

        container.RegisterSingleton<IPrincipal, HttpContextPrincipal>();
        container.RegisterInstance<BusinessLayer.ILogger>(new DebugLogger());

        return container;
    }

    private sealed class HttpContextPrincipal : IPrincipal
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextPrincipal(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IIdentity Identity => this.Principal.Identity!;
        public bool IsInRole(string role) => this.Principal.IsInRole(role);
        private IPrincipal Principal => this.httpContextAccessor.HttpContext?.User!;
    }

    private sealed class DebugLogger : BusinessLayer.ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
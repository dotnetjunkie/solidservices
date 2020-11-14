namespace WebCoreService
{
    using System.Diagnostics;
    using System.Security.Principal;
    using BusinessLayer;
    using Microsoft.AspNetCore.Http;

    public class Bootstrapper : BusinessLayerBootstrapper
    {
        public Bootstrapper(IHttpContextAccessor accessor) : base(
            // ASP.NET Core-specific Singletons
            logger: new DebugLogger(),
            principal: new HttpContextPrincipal(accessor))
        {
        }

        private sealed class HttpContextPrincipal : IPrincipal
        {
            private readonly IHttpContextAccessor httpContextAccessor;

            public HttpContextPrincipal(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }

            public IIdentity Identity => this.Principal.Identity;
            public bool IsInRole(string role) => this.Principal.IsInRole(role);
            private IPrincipal Principal => this.httpContextAccessor.HttpContext.User;
        }

        private sealed class DebugLogger : ILogger
        {
            public void Log(string message) => Debug.WriteLine(message);
        }
    }
}
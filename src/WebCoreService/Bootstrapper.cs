namespace WebCoreService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.Threading;
    using BusinessLayer;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class Bootstrapper
    {
        public static IEnumerable<Type> GetKnownCommandTypes() => BusinessLayerBootstrapper.GetCommandTypes();

        public static IEnumerable<QueryInfo> GetKnownQueryTypes() => BusinessLayerBootstrapper.GetQueryTypes();

        public static Container Bootstrap(Container container, IApplicationBuilder app)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            BusinessLayerBootstrapper.Bootstrap(container);

            container.RegisterSingleton<IPrincipal, HttpContextPrincipal>();
            container.RegisterInstance<ILogger>(new DebugLogger());

            container.Verify();

            return container;
        }

        private sealed class HttpContextPrincipal : IPrincipal
        {
            private readonly IHttpContextAccessor httpContextAccessor;

            public HttpContextPrincipal(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }
            public IIdentity Identity => this.Principal.Identity;
            private IPrincipal Principal => this.httpContextAccessor.HttpContext.User;
            public bool IsInRole(string role) => this.Principal.IsInRole(role);
        }

        private sealed class DebugLogger : ILogger
        {
            public void Log(string message)
            {
                Debug.WriteLine(message);
            }
        }
    }
}

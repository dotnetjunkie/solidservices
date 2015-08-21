namespace WebApiService.CompositionRoot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Http.Dependencies;

    using SimpleInjector;

    public sealed class SimpleInjectorWebApiDependencyResolver : IDependencyResolver
    {
        private readonly Container container;

        [DebuggerStepThrough]
        public SimpleInjectorWebApiDependencyResolver(Container container)
        {
            this.container = container;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)this.container).GetService(serviceType);
        }

        [DebuggerStepThrough]
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.container.GetAllInstances(serviceType);
        }

        public void Dispose()
        {
        }
    }
}
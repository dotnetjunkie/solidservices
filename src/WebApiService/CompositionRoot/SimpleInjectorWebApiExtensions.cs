namespace WebApiService.CompositionRoot
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Compilation;
    using System.Web.Http.Controllers;

    using SimpleInjector;

    public static class SimpleInjectorWebApiExtensions
    {
        public static void RegisterApiControllers(this Container container,
            params Assembly[] assemblies)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            }

            var controllerTypes =
                from assembly in assemblies
                where !assembly.IsDynamic
                from type in assembly.GetExportedTypes()
                where typeof(IHttpController).IsAssignableFrom(type)
                where !type.IsAbstract
                where !type.IsGenericTypeDefinition
                where type.Name.EndsWith("Controller", StringComparison.Ordinal)
                select type;

            foreach (var controllerType in controllerTypes)
            {
                container.Register(controllerType);
            }
        }
    }
}
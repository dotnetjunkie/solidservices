namespace WebApiService.Code
{
    using System;
    using System.Reflection;
    using System.Web.Http.Controllers;

    public class CommandHttpActionSelector : ApiControllerActionSelector
    {
        private readonly CommandControllerDescriptorProvider controllerDescriptorProvider;

        public CommandHttpActionSelector(CommandControllerDescriptorProvider controllerDescriptorProvider)
        {
            this.controllerDescriptorProvider = controllerDescriptorProvider;
        }

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var descriptor = this.controllerDescriptorProvider.GetControllerDescriptorInfo(
                controllerContext.ControllerDescriptor);

            string requestMethod = controllerContext.Request.Method.Method;

            if (descriptor != null && descriptor.Method.ToLowerInvariant() == requestMethod.ToLowerInvariant())
            {
                Type controllerType = controllerContext.ControllerDescriptor.ControllerType;

                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor,
                    controllerType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
            }

            // No command, no query: let's fall back on the default action selection behavior.
            return base.SelectAction(controllerContext);
        }
    }
}
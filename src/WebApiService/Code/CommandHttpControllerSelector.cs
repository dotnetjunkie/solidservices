namespace WebApiService.Code
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    public class CommandHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly CommandControllerDescriptorProvider controllerDescriptorProvider;

        public CommandHttpControllerSelector(CommandControllerDescriptorProvider controllerDescriptorProvider)
            : base(GlobalConfiguration.Configuration)
        {
            this.controllerDescriptorProvider = controllerDescriptorProvider;
        }

        public override IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return base.GetControllerMapping();
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            string controllerName = this.GetControllerName(request);

            string action = (request.GetRouteData().Values["action"] ?? string.Empty).ToString();

            var descriptor = this.controllerDescriptorProvider
                .GetControllerDescriptor(controllerName, action, request.Method.Method);

            if (descriptor != null)
            {
                return descriptor.ControllerDescriptor;
            }

            return base.SelectController(request);
        }
    }
}
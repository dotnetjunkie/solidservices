using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using Contract.Commands.Orders;
using SolidServices.Controllerless.WebApi.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebApiService;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApiService
{
    public static class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            var contractAssembly = typeof(CreateOrderCommand).Assembly;

            GlobalConfiguration.Configuration.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "SOLID Services API");

                string xmlCommentsPath = HostingEnvironment.MapPath("~/App_Data/Contract.xml");

                c.IncludeXmlComments(xmlCommentsPath);

                var filter = new ControllerlessActionOperationFilter(xmlCommentsPath);
                c.OperationFilter(() => filter);
            })
            .EnableSwaggerUi(c => { });
        }

        private sealed class ControllerlessActionOperationFilter : IOperationFilter
        {
            private readonly ITypeDescriptionProvider provider;

            public ControllerlessActionOperationFilter(string xmlCommentsPath)
            {
                this.provider = new XmlDocumentationTypeDescriptionProvider(xmlCommentsPath);
            }

            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                var descriptor = apiDescription.ActionDescriptor as ControllerlessActionDescriptor;

                operation.summary = descriptor != null
                    ? this.provider.GetDescription(descriptor.MessageType)
                    : operation.summary;
            }
        }
    }
}
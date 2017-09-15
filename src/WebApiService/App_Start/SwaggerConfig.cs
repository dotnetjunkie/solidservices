using System.Configuration;
using System.IO;
using System.Linq;
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
    // NOTE: To see Swagger in action, view this Web API in your browser: http://localhost:2591/swagger/
    public static class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            var contractAssembly = typeof(CreateOrderCommand).Assembly;

            GlobalConfiguration.Configuration.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "SOLID Services API");

                IncludeXmlCommentsFromAppDataFolder(c);
            })
            .EnableSwaggerUi(c => { });
        }

        private static void IncludeXmlCommentsFromAppDataFolder(SwaggerDocsConfig c)
        {
            var appDataPath = HostingEnvironment.MapPath("~/App_Data");
            
            string[] paths = Directory.GetFiles(appDataPath, "*.xml");

            foreach (string xmlCommentsPath in paths)
            {
                IncludeXmlComments(c, xmlCommentsPath);
            }

            if (!paths.Any())
            {
                throw new ConfigurationErrorsException("No .xml files were found in the App_Data folder.");
            }
        }

        private static void IncludeXmlComments(SwaggerDocsConfig c, string xmlCommentsPath)
        {
            c.IncludeXmlComments(xmlCommentsPath);
            var filter = new ControllerlessActionOperationFilter(xmlCommentsPath);
            c.OperationFilter(() => filter);
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
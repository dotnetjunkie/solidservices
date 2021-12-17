[assembly: System.Web.PreApplicationStartMethod(typeof(WebApiService.SwaggerConfig), "Register")]
namespace WebApiService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Contract.Commands.Orders;
    using SolidServices.Controllerless.WebApi.Description;
    using Swashbuckle.Application;
    using Swashbuckle.Swagger;

    // NOTE: To see Swagger in action, view this Web API in your browser: http://localhost:2591/swagger/
    public static class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            var contractAssembly = typeof(CreateOrder).Assembly;

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

            // The XML comment files are copied using a post-build event (see project settings / Build Events).
            string[] xmlCommentsPaths = Directory.GetFiles(appDataPath, "*.xml");

            foreach (string xmlCommentsPath in xmlCommentsPaths)
            {
                c.IncludeXmlComments(xmlCommentsPath);
            }

            var filter = new ControllerlessActionOperationFilter(xmlCommentsPaths);
            c.OperationFilter(() => filter);

            if (!xmlCommentsPaths.Any())
            {
                throw new ConfigurationErrorsException("No .xml files were found in the App_Data folder.");
            }
        }

        private sealed class ControllerlessActionOperationFilter : IOperationFilter
        {
            private readonly ITypeDescriptionProvider[] providers;

            public ControllerlessActionOperationFilter(params string[] xmlCommentsPaths)
            {
                this.providers = xmlCommentsPaths.Select(p => new XmlDocumentationTypeDescriptionProvider(p)).ToArray();
            }

            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                var descriptor = apiDescription.ActionDescriptor as ControllerlessActionDescriptor;

                if (descriptor != null)
                {
                    operation.summary = this.GetSummaries(descriptor.MessageType).FirstOrDefault() ?? operation.summary;
                }
            }

            private IEnumerable<string> GetSummaries(Type type) =>
                from provider in providers
                let description = provider.GetDescription(type)
                where description != null
                select description;
        }
    }
}
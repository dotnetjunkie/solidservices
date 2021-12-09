using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebCoreMinimalApiService
{
    public static class SwaggerExtensions
    {
        public static void IncludeXmlDocumentationFromDirectory(this SwaggerGenOptions options, string appDataPath)
        {
            // The XML comment files are copied using a post-build event (see project settings / Build Events).
            string[] xmlCommentsPaths = Directory.GetFiles(appDataPath, "*.xml");

            if (!xmlCommentsPaths.Any())
            {
                throw new InvalidOperationException("No .xml files were found in the App_Data folder.");
            }

            foreach (string xmlCommentsPath in xmlCommentsPaths)
            {
                options.IncludeXmlComments(xmlCommentsPath);
            }
        }

        // The query and command types are the operations, but Swagger nor Web API knows this. This method extracts the
        // summary from class (if available) and places it on the operation.
        public static void IncludeMessageSummariesFromXmlDocs(this SwaggerGenOptions options, string appDataPath)
        {
            string[] xmlCommentsPaths = Directory.GetFiles(appDataPath, "*.xml");

            options.OperationFilter<AddMessageSummaryOperationFilter>(new object[] { xmlCommentsPaths });
        }

        public sealed class AddMessageSummaryOperationFilter : IOperationFilter
        {
            private readonly XmlDocumentationTypeDescriptionProvider[] providers;

            public AddMessageSummaryOperationFilter(string[] xmlCommentsPaths)
            {
                this.providers = xmlCommentsPaths.Select(p => new XmlDocumentationTypeDescriptionProvider(p)).ToArray();
            }

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var api = context.ApiDescription;
                var type = context.ApiDescription.ParameterDescriptions.LastOrDefault()?.Type;

                if (type != null)
                {
                    operation.Summary = this.GetSummaries(type).FirstOrDefault() ?? operation.Summary;
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
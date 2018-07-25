namespace WebCoreService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessLayer;
    using Contract;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using SimpleInjector;

    public sealed class QueryHandlerMiddleware : IMiddleware
    {
        private readonly Func<Type, object> handlerFactory;
        private readonly Dictionary<string, QueryInfo> queryTypes;

        public QueryHandlerMiddleware(Container container)
        {
            this.handlerFactory = container.GetInstance;
            this.queryTypes = Bootstrapper.GetKnownQueryTypes().ToDictionary(
                info => info.QueryType.Name.Replace("Query", string.Empty),
                info => info,
                StringComparer.OrdinalIgnoreCase);
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            HttpRequest request = context.Request;

            string queryName = GetQueryName(request);

            if (this.queryTypes.ContainsKey(queryName))
            {
                // GET operations get their data through the query string, while POST operations expect a JSON
                // object being put in the body.
                string queryData = request.Method.Equals("get", StringComparison.OrdinalIgnoreCase)
                    ? SerializationHelpers.ConvertQueryStringToJson(request.QueryString.Value)
                    : request.Body.ReadToEnd();

                QueryInfo info = this.queryTypes[queryName];

                Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(info.QueryType, info.ResultType);

                this.ApplyHeaders(request);

                dynamic handler = this.handlerFactory.Invoke(handlerType);

                try
                {
                    dynamic query = JsonConvert.DeserializeObject(queryData, info.QueryType);//TODO - need to get the JSONSerializer settings object from somewhere

                    object result = handler.Handle(query);

                    await context.WriteResultAsync(new ObjectResult(result));
                }
                catch (Exception exception)
                {
                    ObjectResult response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception, request);
                    if (response != null)
                    {
                        await context.WriteResultAsync(response);
                    }

                    throw;
                }
            }
            else
                await context.WriteResultAsync(new NotFoundObjectResult(queryName));
        }

        private void ApplyHeaders(HttpRequest request)
        {
            // TODO: Here you read the relevant headers and check them or apply them to the current scope
            // so the values are accessible during execution of the query.
            string sessionId = request.Headers.GetValueOrNull("sessionId");
            string token = request.Headers.GetValueOrNull("CSRF-token");
        }


        private static string GetQueryName(HttpRequest request)
        {
            Uri requestUri = new Uri(request.GetEncodedUrl());
            string segment = requestUri.Segments.LastOrDefault();

            return segment;
        }
    }
}

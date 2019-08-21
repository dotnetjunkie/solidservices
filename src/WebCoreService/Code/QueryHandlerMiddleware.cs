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
        private static readonly Dictionary<string, QueryInfo> QueryTypes;
        private readonly Func<Type, object> handlerFactory;
        private readonly JsonSerializerSettings jsonSettings;

        static QueryHandlerMiddleware()
        {
            QueryTypes = Bootstrapper.GetKnownQueryTypes().ToDictionary(
                info => info.QueryType.Name.Replace("Query", string.Empty),
                info => info,
                StringComparer.OrdinalIgnoreCase);
        }

        public QueryHandlerMiddleware(Container container, JsonSerializerSettings jsonSettings)
        {
            this.handlerFactory = container.GetInstance;
            this.jsonSettings = jsonSettings;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            HttpRequest request = context.Request;

            string queryName = GetQueryName(request);

            if (QueryTypes.ContainsKey(queryName))
            {
                // GET operations get their data through the query string, while POST operations expect a JSON
                // object being put in the body.
                string queryData = request.Method.Equals("get", StringComparison.OrdinalIgnoreCase)
                    ? SerializationHelpers.ConvertQueryStringToJson(request.QueryString.Value)
                    : request.Body.ReadToEnd();

                QueryInfo info = QueryTypes[queryName];

                Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(info.QueryType, info.ResultType);

                this.ApplyHeaders(request);

                dynamic handler = this.handlerFactory.Invoke(handlerType);

                try
                {
                    dynamic query = JsonConvert.DeserializeObject(
                        string.IsNullOrWhiteSpace(queryData) ? "{}" : queryData,
                        info.QueryType,
                        this.jsonSettings);

                    object result = handler.Handle(query);

                    await context.WriteResultAsync(new ObjectResult(result));
                }
                catch (Exception exception)
                {
                    ObjectResult response =
                        WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception, context);

                    if (response != null)
                    {
                        await context.WriteResultAsync(response);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                var response = new ObjectResult(queryName)
                {
                    StatusCode = StatusCodes.Status404NotFound
                };

                await context.WriteResultAsync(response);
            }
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
            return requestUri.Segments.LastOrDefault();
        }
    }
}
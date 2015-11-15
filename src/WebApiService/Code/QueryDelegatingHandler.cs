namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;
    using BusinessLayer;
    using Contract;
    using Newtonsoft.Json;

    public sealed class QueryDelegatingHandler : DelegatingHandler
    {
        public const string QueryNameTag = "query";

        private readonly Func<Type, object> serviceLocator;
        private readonly Dictionary<string, QueryInfo> queryTypes;

        public QueryDelegatingHandler(Func<Type, object> serviceLocator, IEnumerable<QueryInfo> queryTypes)
        {
            this.serviceLocator = serviceLocator;
            this.queryTypes = queryTypes.ToDictionary(
                keySelector: info => info.QueryType.Name.Replace("Query", string.Empty),
                elementSelector: info => info,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // GetDependencyScope() calls IDependencyResolver.BeginScope internally.
            request.GetDependencyScope();

            ApplyHeaders(request);

            IHttpRouteData data = request.GetRouteData();

            string queryName = data.Values[QueryNameTag].ToString();

            string queryData = await request.Content.ReadAsStringAsync();

            QueryInfo info = this.queryTypes[queryName];

            Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(info.QueryType, info.ResultType);

            dynamic handler = this.serviceLocator.Invoke(handlerType);

            if (request.Method == HttpMethod.Get)
            {
                return GetExampleMessage(info, request);
            }

            dynamic query = DeserializeQuery(request, queryData, info.QueryType);

            try
            {
                object result = handler.Handle(query);

                return CreateResponse(result, info.ResultType, HttpStatusCode.OK, request);
            }
            catch (Exception ex)
            {
                var response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(ex, request);

                if (response != null)
                {
                    return response;
                }

                throw;
            }
        }

        private void ApplyHeaders(HttpRequestMessage request)
        {
            // TODO: Here you read the relevant headers and and check them or apply them to the current scope
            // so the values are accessible during execution of the query.
            string sessionId = request.Headers.GetValueOrNull("sessionId");
            string token = request.Headers.GetValueOrNull("CSRF-token");
        }

        private HttpResponseMessage GetExampleMessage(QueryInfo info, HttpRequestMessage request)
        {
            object query = ExampleObjectCreator.GetSampleInstance(info.QueryType);
            object result = ExampleObjectCreator.GetSampleInstance(info.ResultType);

            var data = new { query, result };

            return CreateResponse(data, data.GetType(), HttpStatusCode.MethodNotAllowed, request);
        }

        private static HttpResponseMessage CreateResponse(object data, Type dataType, HttpStatusCode code,
            HttpRequestMessage request)
        {
            return new HttpResponseMessage
            {
                Content = new ObjectContent(dataType, data, GetJsonFormatter(request)),
                StatusCode = code,
                RequestMessage = request
            };
        }

        private static dynamic DeserializeQuery(HttpRequestMessage request, string json, Type queryType) =>
            JsonConvert.DeserializeObject(json, queryType, GetJsonFormatter(request).SerializerSettings);

        private static JsonMediaTypeFormatter GetJsonFormatter(HttpRequestMessage request) => 
            request.GetConfiguration().Formatters.JsonFormatter;
    }
}
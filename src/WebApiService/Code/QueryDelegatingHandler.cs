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

            IHttpRouteData data = request.GetRouteData();

            string queryName = data.Values[QueryNameTag].ToString();

            string queryData = await request.Content.ReadAsStringAsync();

            QueryInfo info = this.queryTypes[queryName];

            Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(info.QueryType, info.ResultType);

            dynamic query = JsonConvert.DeserializeObject(queryData, info.QueryType);

            dynamic handler = this.serviceLocator.Invoke(handlerType);

            var formatter = request.GetConfiguration().Formatters.OfType<JsonMediaTypeFormatter>().First();

            if (request.Method == HttpMethod.Get)
            {
                return GetExampleMessage(info, request);
            }

            try
            {
                object result = handler.Handle(query);

                return new HttpResponseMessage
                {
                    Content = new ObjectContent(info.ResultType, result, formatter),
                    StatusCode = HttpStatusCode.OK,
                    RequestMessage = request
                };
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

        private HttpResponseMessage GetExampleMessage(QueryInfo info, HttpRequestMessage request)
        {
            object query = ExampleObjectCreator.GetSampleInstance(info.QueryType);
            object result = ExampleObjectCreator.GetSampleInstance(info.ResultType);

            var data = new { query, result };

            return new HttpResponseMessage
            {
                Content = new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter { Indent = true }),
                StatusCode = HttpStatusCode.MethodNotAllowed,
                RequestMessage = request
            };
        }
    }
}
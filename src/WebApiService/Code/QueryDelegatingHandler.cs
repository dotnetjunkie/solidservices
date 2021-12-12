﻿namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Contract;
    using Newtonsoft.Json;

    public sealed class QueryDelegatingHandler : DelegatingHandler
    {
        private readonly Func<Type, object> handlerFactory;
        private readonly Dictionary<string, (Type QueryType, Type ResultType)> queryTypes;

        public QueryDelegatingHandler(
            Func<Type, object> handlerFactory, IEnumerable<(Type QueryType, Type ResultType)> queryTypes)
        {
            this.handlerFactory = handlerFactory;
            this.queryTypes = queryTypes.ToDictionary(
                keySelector: info => info.QueryType.Name.Replace("Query", string.Empty),
                elementSelector: info => info,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string queryName = request.GetRouteData().Values["query"].ToString();

            if (!this.queryTypes.ContainsKey(queryName))
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, RequestMessage = request };
            }

            // GET operations get their data through the query string, while POST operations expect a JSON
            // object being put in the body.
            string queryData = request.Method == HttpMethod.Get
                ? SerializationHelpers.ConvertQueryStringToJson(request.RequestUri.Query)
                : await request.Content.ReadAsStringAsync();

            var (queryType, resultType) = this.queryTypes[queryName];

            Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);

            // GetDependencyScope() calls IDependencyResolver.BeginScope internally.
            request.GetDependencyScope();

            this.ApplyHeaders(request);

            dynamic handler = this.handlerFactory.Invoke(handlerType);

            try
            {
                dynamic query = DeserializeQuery(request, queryData, queryType);

                object result = handler.Handle(query);

                return CreateResponse(result, resultType, HttpStatusCode.OK, request);
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
            // TODO: Here you read the relevant headers and check them or apply them to the current scope
            // so the values are accessible during execution of the query.
            string sessionId = request.Headers.GetValueOrNull("sessionId");
            string token = request.Headers.GetValueOrNull("CSRF-token");
        }

        private static HttpResponseMessage CreateResponse(object data, Type dataType, HttpStatusCode code,
            HttpRequestMessage request)
        {
            var configuration = request.GetConfiguration();

            IContentNegotiator negotiator = configuration.Services.GetContentNegotiator();
            ContentNegotiationResult result = negotiator.Negotiate(dataType, request, configuration.Formatters);

            var bestMatchFormatter = result.Formatter;
            var mediaType = result.MediaType.MediaType;

            return new HttpResponseMessage
            {
                Content = new ObjectContent(dataType, data, bestMatchFormatter, mediaType),
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
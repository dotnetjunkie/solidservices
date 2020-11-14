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
    using BusinessLayer;
    using Newtonsoft.Json;

    public sealed class CommandDelegatingHandler : DelegatingHandler
    {
        private readonly Func<Type, object> handlerFactory;
        private readonly Dictionary<string, Type> commandTypes;

        public CommandDelegatingHandler(Func<Type, object> handlerFactory, IEnumerable<Type> commandTypes)
        {
            this.handlerFactory = handlerFactory;
            this.commandTypes = commandTypes.ToDictionary(
                keySelector: type => type.Name.Replace("Command", string.Empty),
                elementSelector: type => type,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string commandName = request.GetRouteData().Values["command"].ToString();

            if (request.Method != HttpMethod.Post)
            {
                return request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed,
                    "The requested resource does not support http method '" + request.Method + "'.");
            }

            if (!this.commandTypes.ContainsKey(commandName))
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, RequestMessage = request };
            }

            Type commandType = this.commandTypes[commandName];

            string commandData = await request.Content.ReadAsStringAsync();

            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            // GetDependencyScope() calls IDependencyResolver.BeginScope internally.
            request.GetDependencyScope();

            this.ApplyHeaders(request);

            dynamic handler = this.handlerFactory.Invoke(commandType);

            try
            {
                dynamic command = DeserializeCommand(request, commandData, commandType);

                handler.Handle(command);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, RequestMessage = request };
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
            // so the values are accessible during execution of the command.
            string sessionId = request.Headers.GetValueOrNull("sessionId");
            string token = request.Headers.GetValueOrNull("CSRF-token");
        }

        private static object DeserializeCommand(HttpRequestMessage request, string json, Type commandType) =>
            JsonConvert.DeserializeObject(json, commandType, GetJsonFormatter(request).SerializerSettings);

        private static JsonMediaTypeFormatter GetJsonFormatter(HttpRequestMessage request) => 
            request.GetConfiguration().Formatters.JsonFormatter;
    }
}
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
    using Contract;
    using Newtonsoft.Json;

    public sealed class CommandDelegatingHandler : DelegatingHandler
    {
        public const string CommandNameTag = "command";

        private readonly Func<Type, object> serviceLocator;
        private readonly Dictionary<string, Type> commandTypes;

        public CommandDelegatingHandler(Func<Type, object> serviceLocator, IEnumerable<Type> commandTypes)
        {
            this.serviceLocator = serviceLocator;
            this.commandTypes = commandTypes.ToDictionary(
                keySelector: type => type.Name.Replace("Command", string.Empty),
                elementSelector: type => type,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // GetDependencyScope() calls IDependencyResolver.BeginScope internally.
            request.GetDependencyScope();

            IHttpRouteData data = request.GetRouteData();

            string commandName = data.Values[CommandNameTag].ToString();

            string commandData = await request.Content.ReadAsStringAsync();

            Type commandType = this.commandTypes[commandName];

            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            dynamic command = JsonConvert.DeserializeObject(commandData, commandType);

            dynamic handler = this.serviceLocator.Invoke(handlerType);

            if (request.Method == HttpMethod.Get)
            {
                return GetExampleMessage(commandType, request);
            }

            try
            {
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

        private static HttpResponseMessage GetExampleMessage(Type commandType, HttpRequestMessage request)
        {
            object command = ExampleObjectCreator.GetSampleInstance(commandType);

            return new HttpResponseMessage
            {
                Content = new ObjectContent(commandType, command, new JsonMediaTypeFormatter { Indent = true }),
                StatusCode = HttpStatusCode.MethodNotAllowed,
                RequestMessage = request
            };
        }
    }
}
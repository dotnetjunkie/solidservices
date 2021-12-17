namespace WebCoreService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessLayer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using SimpleInjector;

    public sealed class CommandHandlerMiddleware : IMiddleware
    {
        private static readonly Dictionary<string, Type> CommandTypes;

        private readonly Func<Type, object> handlerFactory;
        private readonly JsonSerializerSettings jsonSettings;

        static CommandHandlerMiddleware()
        {
            CommandTypes = Bootstrapper.GetKnownCommandTypes().ToDictionary(
                keySelector: type => type.ToFriendlyName(),
                elementSelector: type => type,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        public CommandHandlerMiddleware(Container container, JsonSerializerSettings jsonSettings)
        {
            this.handlerFactory = container.GetInstance;
            this.jsonSettings = jsonSettings;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            HttpRequest request = context.Request;

            string commandName = GetCommandName(request);

            if (request.Method == "POST" && CommandTypes.ContainsKey(commandName))
            {
                Type commandType = CommandTypes[commandName];

                string commandData = request.Body.ReadToEnd();

                Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

                this.ApplyHeaders(request);

                dynamic handler = this.handlerFactory.Invoke(handlerType);

                try
                {
                    dynamic command = JsonConvert.DeserializeObject(
                        string.IsNullOrWhiteSpace(commandData) ? "{}" : commandData,
                        commandType,
                        this.jsonSettings);

                    handler.Handle(command);

                    var result = new ObjectResult(null);

                    await context.WriteResultAsync(result);
                }
                catch (Exception exception)
                {
                    var response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception, context);

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
                await context.WriteResultAsync(new NotFoundObjectResult(commandName));
            }
        }

        private void ApplyHeaders(HttpRequest request)
        {
            // TODO: Here you read the relevant headers and and check them or apply them to the current scope
            // so the values are accessible during execution of the command.
            string sessionId = request.Headers.GetValueOrNull("sessionId");
            string token = request.Headers.GetValueOrNull("CSRF-token");
        }

        private static string GetCommandName(HttpRequest request)
        {
            Uri requestUri = new Uri(request.GetEncodedUrl());
            return requestUri.Segments.LastOrDefault();
        }
    }
}
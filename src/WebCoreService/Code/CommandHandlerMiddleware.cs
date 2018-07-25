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
        private readonly Func<Type, object> handlerFactory;
        private readonly Dictionary<string, Type> commandTypes;

        public CommandHandlerMiddleware(Container container)
        {
            this.handlerFactory = container.GetInstance;
            this.commandTypes = Bootstrapper.GetKnownCommandTypes().ToDictionary(
                keySelector: type => type.Name.Replace("Command", string.Empty),
                elementSelector: type => type,
                comparer: StringComparer.OrdinalIgnoreCase);
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            HttpRequest request = context.Request;

            string commandName = GetCommandName(request);

            if (this.commandTypes.ContainsKey(commandName))
            {
                Type commandType = this.commandTypes[commandName];

                string commandData = request.Body.ReadToEnd();

                Type handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

                this.ApplyHeaders(request);

                dynamic handler = this.handlerFactory.Invoke(handlerType);

                try
                {
                    dynamic command = JsonConvert.DeserializeObject(commandData, commandType);//TODO - need to get the JSONSerializer settings object from somewhere

                    handler.Handle(command);

                    EmptyResult result = new EmptyResult();//TODO - might need to change to ObjectResult of some type see comment in HttpContextExtsions
                    
                    await context.WriteResultAsync(result);
                }
                catch (Exception exception)
                {
                    var response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception, request);
                    if (response != null)
                    {
                        await context.WriteResultAsync(response);
                    }

                    throw;
                }
            }
            else
                await context.WriteResultAsync(new NotFoundObjectResult(commandName));

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
            string segment = requestUri.Segments.LastOrDefault();

            return segment;
        }
    }
}
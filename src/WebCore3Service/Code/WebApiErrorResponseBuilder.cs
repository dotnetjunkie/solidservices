namespace WebCoreService.Code
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Newtonsoft.Json;

    public static class WebApiErrorResponseBuilder
    {
        // Allows translating exceptions thrown by the business layer to HttpResponseExceptions. 
        // This allows returning useful error information to the client.
        public static ObjectResult CreateErrorResponseOrNull(Exception thrownException, HttpContext context)
        {
            // TODO: context.Response.ContentType is always null, not sure why.
            var contentTypes = new MediaTypeCollection { context.Response.ContentType ?? "application/json" };

            // Here are some examples of how certain exceptions can be mapped to error responses.
            switch (thrownException)
            {
                case JsonException _:
                    // Return when the supplied model (command or query) can't be deserialized.
                    return new BadRequestObjectResult(thrownException.Message) { ContentTypes = contentTypes };

                case ValidationException exception:
                    // Return when the supplied model (command or query) isn't valid.
                    return new BadRequestObjectResult(exception.ValidationResult) { ContentTypes = contentTypes };

                // case OptimisticConcurrencyException _:
                //     // Return when there was a concurrency conflict in updating the model.
                //     return new ConflictObjectResult(thrownException.Message) { ContentTypes = contentTypes };

                case SecurityException _:
                    // Return when the current user doesn't have the proper rights to execute the requested
                    // operation or to access the requested resource.
                    return new ObjectResult(null)
                    {
                        ContentTypes = contentTypes,
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };

                case KeyNotFoundException _:
                    // Return when the requested resource does not exist anymore. Catching a KeyNotFoundException 
                    // is an example, but you probably shouldn't throw KeyNotFoundException in this case, since it
                    // could be thrown for other reasons (such as program errors) in which case this branch should
                    // of course not execute.
                    return new NotFoundObjectResult(thrownException.Message) { ContentTypes = contentTypes };
            }

            // If the thrown exception can't be handled: return null.
            return null;
        }
    }
}
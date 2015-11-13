namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Net;
    using System.Net.Http;
    using System.Security;

    // Allows translating exceptions thrown by the business layer to HttpResponseExceptions. 
    // This allows returning useful error information to the client.
    public static class WebApiErrorResponseBuilder
    {
        public static HttpResponseMessage CreateErrorResponseOrNull(Exception thrownException, 
            HttpRequestMessage request)
        {
            // Here are some examples of how certain exceptions can be mapped to error responses.
            if (thrownException is ValidationException)
            {
                // Return when the supplied model (command or query) isn't valid.
                return request.CreateResponse<ValidationResult>(HttpStatusCode.BadRequest,
                    ((ValidationException)thrownException).ValidationResult);
            }

            if (thrownException is OptimisticConcurrencyException)
            {
                // Return when there was a concurrency conflict in updating the model.
                return request.CreateErrorResponse(HttpStatusCode.Conflict, thrownException);
            }

            if (thrownException is SecurityException)
            {
                // Return when the current user doesn't have the proper rights to execute the requested
                // operation or to access the requested resource.
                return request.CreateErrorResponse(HttpStatusCode.Unauthorized, thrownException);
            }

            if (thrownException is KeyNotFoundException)
            {
                // Return when the requested resource does not exist anymore. Catching a KeyNotFoundException 
                // is an example, but you probably shouldn't throw KeyNotFoundException in this case, since it
                // could be thrown for other reasons (such as program errors) in which case this branch should
                // of course not execute.
                return request.CreateErrorResponse(HttpStatusCode.NotFound, thrownException);
            }

            // If the thrown exception can't be handled: return null.
            return null;
        }
    }
}
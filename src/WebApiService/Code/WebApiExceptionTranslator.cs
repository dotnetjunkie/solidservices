namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Net;
    using System.Net.Http;
    using BusinessLayer.CrossCuttingConcerns;

    // Allows translating exceptions thrown by the business layer to HttpResponseExceptions. This allows returning
    // useful error information to the client.
    public static class WebApiErrorResponseBuilder
    {
        public static HttpResponseMessage CreateErrorResponse(Exception throwException, HttpRequestMessage request)
        {
            // Here are some examples of how certain exceptions can be mapped to error responses.
            if (throwException is System.ComponentModel.DataAnnotations.ValidationException)
            {
                // Return when the supplied model (command or query) isn't valid.
                return request.CreateResponse<ValidationResult>(HttpStatusCode.BadRequest,
                    ((ValidationException)throwException).ValidationResult);
            }

            if (throwException is OptimisticConcurrencyException)
            {
                // Return when there was a concurrency conflict in updating the model.
                return request.CreateErrorResponse(HttpStatusCode.Conflict, throwException);
            }

            if (throwException is BusinessLayer.CrossCuttingConcerns.AuthorizationException)
            {
                // Return when the current user doesn't have the proper rights to execute the requested operation
                // or to access the requested resource.
                return request.CreateErrorResponse(HttpStatusCode.Unauthorized, throwException);
            }

            if (throwException is System.Collections.Generic.KeyNotFoundException)
            {
                // Return when the requested resource does not exist anymore. Catching a KeyNotFoundException 
                // is an example, but you probably shouldn't throw KeyNotFoundException in this case, since it
                // could be thrown for other reasons (such as program errors) in which case this branch should
                // of course not execute.
                return request.CreateErrorResponse(HttpStatusCode.NotFound, throwException);
            }

            // If the throwException can't be handled: return null.
            return null;
        }
    }
}
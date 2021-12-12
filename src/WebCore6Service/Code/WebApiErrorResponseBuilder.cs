namespace WebCoreService;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security;

public static class WebApiErrorResponseBuilder
{
    // Allows translating exceptions thrown by the business layer to HttpResponseExceptions. 
    // This allows returning useful error information to the client.
    public static IResult? CreateErrorResponseOrNull(Exception thrownException)
    {
        // Here are some examples of how certain exceptions can be mapped to error responses.
        switch (thrownException)
        {
            case JsonException:
                // Return when the supplied model (command or query) can't be deserialized.
                return Results.BadRequest(thrownException.Message);

            case ValidationException exception:
                // Return when the supplied model (command or query) isn't valid.
                return Results.BadRequest(exception.ValidationResult);

            case SecurityException:
                // Return when the current user doesn't have the proper rights to execute the requested
                // operation or to access the requested resource.
                return Results.Unauthorized();

            case KeyNotFoundException:
                // Return when the requested resource does not exist anymore. Catching a KeyNotFoundException 
                // is an example, but you probably shouldn't throw KeyNotFoundException in this case, since it
                // could be thrown for other reasons (such as program errors) in which case this branch should
                // of course not execute.
                return Results.NotFound(thrownException.Message);

            default:
                // If the thrown exception can't be handled: return null.
                return null;
        }
    }
}
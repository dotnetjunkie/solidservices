using BusinessLayer;
using SimpleInjector;
using System.Reflection;

namespace WebCoreService;

public static class MessageMappingExtensions
{
    public static void MapCommands(
        this IEndpointRouteBuilder app, string patternFormat, Container container, IEnumerable<Type> commandTypes)
    {
        var dispatcher = new Commands(container);
        MethodInfo genericCommandMethod = typeof(Commands).GetMethod(nameof(Commands.InvokeAsync))!;

        foreach (Type commandType in commandTypes)
        {
            MethodInfo method = genericCommandMethod.MakeGenericMethod(commandType);
            Type funcType = typeof(Func<,>).MakeGenericType(
                method.GetParameters().Append(method.ReturnParameter).Select(p => p.ParameterType).ToArray());
            Delegate handler = Delegate.CreateDelegate(funcType, dispatcher, method);
            var commandName = commandType.Name.Replace("Command", string.Empty);
            var pattern = string.Format(patternFormat, commandName);
            app.MapPost(pattern, handler);
        }
    }

    public static void MapQueries(
        this IEndpointRouteBuilder app, string patternFormat, Container container, IEnumerable<QueryInfo> queryTypes)
    {
        var dispatcher = new Queries(container);
        MethodInfo genericMethod = typeof(Queries).GetMethod(nameof(Queries.InvokeAsync))!;

        foreach (QueryInfo info in queryTypes)
        {
            MethodInfo method = genericMethod.MakeGenericMethod(info.QueryType, info.ResultType);
            Type funcType = typeof(Func<,,>).MakeGenericType(
                method.GetParameters().Append(method.ReturnParameter).Select(p => p.ParameterType).ToArray());
            Delegate handler = Delegate.CreateDelegate(funcType, dispatcher, method);
            var queryName = info.QueryType.Name.Replace("Query", string.Empty);
            var pattern = string.Format(patternFormat, queryName);

            // Hi, dear reader. I need your help. This method registers a query call as a HTTP POST action.  
            // This might be fine for some APIs, others might require the query object to be called as HTTP
            // GET, while its arguments are serialized as part of the URL query string. This isn't supported
            // at the moment. To give an example, using the POST operation, the data for the
            // GetUnshippedOrdersForCurrentCustomerQuery query is serialized in the HTTP body, as the
            // { Paging { PageIndex = 3, PageSize = 10 } } JSON string. Using GET and the query string
            // instead, the request could look as follows:
            // /api/queries/GetUnshippedOrdersForCurrentCustomer?Paging.PageIndex=3&Paging.PageSize=10.
            // The WebCoreService project actually contains a SerializationHelpers that allows deserializing
            // a query string back to a DTO, but there isn't any support for Swagger in there. At this point,
            // it's unclear to me how to achieve this using the new ASP.NET Core Minimal API, while
            //   1. (preferably) keeping the implementation simple, and
            //   2. allowing this without the need for any query-specific code, and
            //   3. allowing this to integrate nicely in the Swagger and API Explorer.
            // If you have any suggestions, you can send me a pull request, or start a conversation here:
            // https://github.com/dotnetjunkie/solidservices/issues/new.
            app.MapPost(pattern, handler);
        }
    }
}

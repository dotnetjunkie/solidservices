namespace WebCoreService;

using System.Reflection;

/// <summary>
/// Builds mappings based on a flat model where:
///  * routes are presented as a non-hierarchical (flat) list, e.g.
///    /api/ShipOrder, /api/CancelOrder, /api/GetAllOrders, /api/FormatHardDrive, etc.
///  * only the POST verb is used (i.e. messages are always sent through HTTP Post operations).
/// This is ideal for .NET clients that reuse the assembly (the Contract project), and are not interested in
/// having a rich REST-full API at their disposal.
/// </summary>
public sealed class FlatApiMessageMappingBuilder : IMessageMappingBuilder
{
    private readonly string patternFormat;
    private readonly object dispatcher;
    private readonly MethodInfo genericMethod;

    public FlatApiMessageMappingBuilder(object dispatcher, string patternFormat = "/api/{0}")
    {
        this.patternFormat = patternFormat;
        this.dispatcher = dispatcher;
        this.genericMethod = dispatcher.GetType().GetMethod("InvokeAsync")
            ?? throw new ArgumentException("InvokeAsync method is missing.");
    }

    public (string, string[], Delegate) BuildMapping(Type messageType, Type? returnType)
    {
        (Type[] GenericArguments, Type GenericFuncType) args = returnType != null
            ? (new[] { messageType, returnType }, typeof(Func<,,>))
            : (new[] { messageType }, typeof(Func<,>));

        MethodInfo method = this.genericMethod.MakeGenericMethod(args.GenericArguments);

        Type funcType = args.GenericFuncType.MakeGenericType(
            method.GetParameters().Append(method.ReturnParameter).Select(p => p.ParameterType).ToArray());

        Delegate handler = Delegate.CreateDelegate(funcType, dispatcher, method);

        var pattern = string.Format(this.patternFormat, GetMessageRoute(messageType));

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
        return (pattern, new[] { HttpMethods.Post }, handler);
    }

    private static string GetMessageRoute(Type messageType) =>

        // ToFriendlyName builds an easy to read type name. Namespaces will be omitted, and generic types
        // will be displayed in a C#-like syntax.
        SimpleInjector.TypesExtensions.ToFriendlyName(messageType)

        // Replace generic markers. Typically they are allowed as root, but that would be frowned upon.
        .Replace("<", string.Empty).Replace(">", string.Empty);
}
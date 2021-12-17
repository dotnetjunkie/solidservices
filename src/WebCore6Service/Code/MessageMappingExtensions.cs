namespace WebCoreService;

public static class MessageMappingExtensions
{
    public static void MapCommands(
        this IEndpointRouteBuilder app,
        IMessageMappingBuilder pattern,
        IEnumerable<Type> commandTypes)
    {
        foreach (Type commandType in commandTypes)
        {
            app.MapMessage(pattern, commandType);
        }
    }

    public static void MapQueries(
        this IEndpointRouteBuilder app,
        IMessageMappingBuilder pattern,
        IEnumerable<(Type QueryType, Type ResultType)> queryTypes)
    {
        foreach (var info in queryTypes)
        {
            app.MapMessage(pattern, info.QueryType, info.ResultType);
        }
    }

    public static void MapMessage(
        this IEndpointRouteBuilder app,
        IMessageMappingBuilder pattern,
        Type messageType,
        Type? returnType = null)
    {
        var mapping = pattern.BuildMapping(messageType, returnType);

        app.MapMethods(mapping.Pattern, mapping.HttpMethods, mapping.Handler);
    }
}
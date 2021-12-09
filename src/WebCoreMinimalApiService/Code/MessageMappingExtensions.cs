namespace WebCoreMinimalApiService;

using BusinessLayer;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            app.MapPost(pattern, handler);
        }
    }
}

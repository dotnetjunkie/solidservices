namespace WebCoreService;

public interface IMessageMappingBuilder
{
    (string Pattern, string[] HttpMethods, Delegate Handler) BuildMapping(Type messageType, Type? returnType = null);
}

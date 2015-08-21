namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using Contract;

    public class CommandControllerDescriptorProvider
    {
        private readonly Dictionary<HttpControllerDescriptor, CommandControllerDescriptor> descriptorMapping;
        private readonly Dictionary<object, CommandControllerDescriptor> descriptorsByNameActionAndMethod;
        private readonly Dictionary<Type, CommandControllerDescriptor> descriptorByMessageType;

        public CommandControllerDescriptorProvider(params Assembly[] assemblies)
        {
            var concreteTypesInAssemblies = (
                from assembly in assemblies
                from type in assembly.GetExportedTypes()
                where !type.IsAbstract && !type.IsGenericTypeDefinition
                select type)
                .ToArray();

            var commandControllerDescriptors =
                from type in concreteTypesInAssemblies
                where type.Name.EndsWith("Command")
                let controllerName = type.Namespace.Replace("Contract.Commands.", string.Empty)
                let descriptor = BuildCommandHttpControllerDescriptor(type, controllerName)
                let action = type.Name.Replace("Command", string.Empty)
                let method = WebApiMethodAttribute.GetMethod(type) ?? "Post"
                select new CommandControllerDescriptor(descriptor, action, type, method);

            var queryControllerDescriptors =
                from type in concreteTypesInAssemblies
                where type.Name.EndsWith("Query")
                let controllerName = type.Namespace.Replace("Contract.Queries.", string.Empty)
                let descriptor = BuildQueryHttpControllerDescriptor(type, controllerName)
                let action = type.Name.Replace("Query", string.Empty)
                let method = WebApiMethodAttribute.GetMethod(type) ?? "Get"
                select new CommandControllerDescriptor(descriptor, action, type, method);

            var descriptors = commandControllerDescriptors.Union(queryControllerDescriptors).ToArray();

            this.descriptorMapping = 
                descriptors.ToDictionary(descriptor => descriptor.ControllerDescriptor, descriptor => descriptor);

            this.descriptorByMessageType =
                descriptors.ToDictionary(descriptor => descriptor.MessageType, descriptor => descriptor);

            this.descriptorsByNameActionAndMethod = (
                from descriptor in descriptors
                select new { descriptor, descriptor.Method })
                .ToDictionary(
                    value => CreateKey(
                        value.descriptor.ControllerDescriptor.ControllerName, 
                        value.descriptor.Action, 
                        value.Method),
                    value => value.descriptor);
        }

        public IEnumerable<CommandControllerDescriptor> GetDescriptors()
        {
            return this.descriptorMapping.Values;
        }

        public CommandControllerDescriptor GetControllerDescriptor(Type messageType)
        {
            CommandControllerDescriptor commandControllerDescriptor;

            if (this.descriptorByMessageType.TryGetValue(messageType, out commandControllerDescriptor))
            {
                return commandControllerDescriptor;
            }

            return null;
        }

        public CommandControllerDescriptor GetControllerDescriptor(string controllerName, string action,
            string method)
        {
            object key = CreateKey(controllerName, action, method);

            CommandControllerDescriptor commandControllerDescriptor;

            if (this.descriptorsByNameActionAndMethod.TryGetValue(key, out commandControllerDescriptor))
            {
                return commandControllerDescriptor;
            }
            
            return null;
        }

        public CommandControllerDescriptor GetControllerDescriptorInfo(
            HttpControllerDescriptor httpControllerDescriptor)
        {
            CommandControllerDescriptor info;
            
            this.descriptorMapping.TryGetValue(httpControllerDescriptor, out info);

            return info;
        }

        private static object CreateKey(string controllerName, string action, string method)
        {
            return new
            {
                ControllerName = controllerName.ToLowerInvariant(),
                Action = action.ToLowerInvariant(),
                Method = method.ToLowerInvariant()
            };
        }

        private static HttpControllerDescriptor BuildCommandHttpControllerDescriptor(Type commandType,
            string controllerName)
        {
            return new HttpControllerDescriptor(GlobalConfiguration.Configuration, controllerName,
                typeof(CommandController<>).MakeGenericType(commandType));
        }

        private static HttpControllerDescriptor BuildQueryHttpControllerDescriptor(Type queryType,
            string controllerName)
        {
            var resultType = GetQueryResultType(queryType);

            return new HttpControllerDescriptor(GlobalConfiguration.Configuration, controllerName,
                typeof(QueryController<,>).MakeGenericType(queryType, resultType));
        }

        private static Type GetQueryResultType(Type queryType)
        {
            return GetQueryInterface(queryType)
                .GetGenericArguments()[0];
        }

        private static Type GetQueryInterface(Type queryType)
        {
            return (
                from @interface in queryType.GetInterfaces()
                where @interface.IsGenericType
                where typeof(IQuery<>).IsAssignableFrom(
                    @interface.GetGenericTypeDefinition())
                select @interface)
                .Single();
        }
    }
}
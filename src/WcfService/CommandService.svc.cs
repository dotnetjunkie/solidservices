namespace WcfService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
       
    using Contract;
    
    using WcfService.CompositionRoot;

    [ServiceContract(Namespace = "http://www.solid.net/commandservice/v1.0")]
    [ServiceKnownType("GetKnownTypes")]
    public class CommandService
    {
        [OperationContract]
        public object Execute(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic commandHandler = Bootstrapper.GetInstance(commandHandlerType);

            commandHandler.Handle(command);

            // Instead of returning the output property of a command, we just return the complete command.
            // There is some overhead in this, but is of course much easier than returning a part of the command.
            return command;
        }

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            var coreAssembly = typeof(ICommandHandler<>).Assembly;

            var commandTypes =
                from type in coreAssembly.GetExportedTypes()
                where type.Name.EndsWith("Command")
                select type;

            return commandTypes.ToArray();
        }
    }
}
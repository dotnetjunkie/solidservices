namespace Client.Code
{
    using System.Diagnostics;
    using System.ServiceModel;
    using Client.Wcf;

    // This service reference is hand-coded. This allows us to use our custom KnownCommandTypesAttribute, 
    // which allows providing WCF with known types at runtime. This prevents us to have to update the client 
    // reference each time a new command is added to the system.
    [KnownCommandTypes]
    [ServiceContract(
        Namespace = "http://www.solid.net/commandservice/v1.0",
        ConfigurationName = "CommandServices.CommandService")]
    public interface CommandService
    {
        [OperationContract(
            Action = "http://www.solid.net/commandservice/v1.0/CommandService/Execute",
            ReplyAction = "http://www.solid.net/commandservice/v1.0/CommandService/ExecuteResponse")]
        object Execute(object command);
    }

    public class CommandServiceClient : ClientBase<CommandService>, CommandService
    {
        [DebuggerStepThrough]
        public object Execute(object command) => this.Channel.Execute(command);
    }
}
namespace Client.Code
{
    using System.Diagnostics;
    using System.ServiceModel;
    using Client.Wcf;

    // This service reference is hand-coded. This allows us to use the KnownQueryAndResultTypesAttribute, 
    // which allows providing WCF with known types at runtime. This prevents us to have to update the client 
    // reference each time a new command is added to the system.
    [KnownQueryAndResultTypes]
    [ServiceContract(
        Namespace = "http://www.cuttingedge.it/solid/queryservice/v1.0",
        ConfigurationName = "QueryServices.QueryService")]
    public interface QueryService
    {
        [OperationContract(
            Action = "http://www.cuttingedge.it/solid/queryservice/v1.0/QueryService/Execute",
            ReplyAction = "http://www.cuttingedge.it/solid/queryservice/v1.0/QueryService/ExecuteResponse")]
        object Execute(object query);
    }

    public class QueryServiceClient : ClientBase<QueryService>, QueryService
    {
        [DebuggerStepThrough]
        public object Execute(object query) => this.Channel.Execute(query);
    }
}
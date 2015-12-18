namespace Client.Wcf
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public abstract class KnownTypesAttribute : Attribute, IContractBehavior
    {
        private readonly KnownTypesDataContractResolver resolver;

        public KnownTypesAttribute(KnownTypesDataContractResolver resolver)
        {
            this.resolver = resolver;
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
            CreateMyDataContractSerializerOperationBehaviors(contractDescription);
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            CreateMyDataContractSerializerOperationBehaviors(contractDescription);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        private void CreateMyDataContractSerializerOperationBehaviors(ContractDescription description)
        {
            foreach (OperationDescription operationDescription in description.Operations)
            {
                CreateMyDataContractSerializerOperationBehavior(operationDescription);
            }
        }

        private void CreateMyDataContractSerializerOperationBehavior(OperationDescription operationDescription)
        {
            DataContractSerializerOperationBehavior dataContractSerializerOperationbehavior =
                operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();

            dataContractSerializerOperationbehavior.DataContractResolver = this.resolver;
        }
    }
}
namespace WcfService.CrossCuttingConcerns
{
    using System.ComponentModel.DataAnnotations;
    using System.ServiceModel;
    using BusinessLayer;
    using Contract;

    public class ToWcfFaultTranslatorCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decoratee;

        public ToWcfFaultTranslatorCommandHandlerDecorator(ICommandHandler<TCommand> decoratee)
        {
            this.decoratee = decoratee;
        }
        
        public void Handle(TCommand command)
        {
            try
            {
                this.decoratee.Handle(command);
            }
            catch (ValidationException ex)
            {
                // This ensures that validation errors are communicated to the client,
                // while other exceptions are filtered by WCF (if configured correctly).
                throw new FaultException(ex.Message, new FaultCode("ValidationError"));
            }
        }
    }
}
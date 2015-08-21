namespace Client.CrossCuttingConcerns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using Contract;

    public class FromWcfFaultTranslatorCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decoratee;

        public FromWcfFaultTranslatorCommandHandlerDecorator(ICommandHandler<TCommand> decoratee)
        {
            this.decoratee = decoratee;
        }

        public void Handle(TCommand command)
        {
            try
            {
                this.decoratee.Handle(command);
            }
            catch (FaultException ex)
            {
                if (ex.Code.Name == "ValidationError")
                {
                    // The WCF service communicates this specific error back to us in case of a validation error.
                    // We translate it back to an exception that the client can handle.
                    throw new ValidationException(ex.Message);
                }

                throw;
            }
        }
    }
}
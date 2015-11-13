namespace Client.Code
{
    using System;
    using Client.CommandServices;

    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        public void Handle(TCommand command)
        {
            var service = new CommandServiceClient();

            try
            {
                service.Execute(command);
            }
            finally
            {
                try
                {
                    ((IDisposable)service).Dispose();
                }
                catch
                {
                    // Against good practice and the Framework Design Guidelines, WCF can through an
                    // exception during a call to Dispose, which can result in loss of the original exception.
                    // See: https://marcgravell.blogspot.com/2008/11/dontdontuse-using.html
                    // See: https://msdn.microsoft.com/en-us/library/aa355056.aspx
                }
            }
        }
    }
}
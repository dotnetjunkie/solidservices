namespace Client.Code
{
    using System.Linq;

    using Client.CommandServices;
    using Contract;

    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        public void Handle(TCommand command)
        {
            using (var service = new CommandServiceClient())
            {
                object result = service.Execute(command);

                Update(source: result, destination: command);
            }
        }

        private static void Update(object source, object destination)
        {
            var properties =
                from property in destination.GetType().GetProperties()
                where property.CanRead && property.CanWrite
                select property;

            foreach (var property in properties)
            {
                object value = property.GetValue(source, null);

                property.SetValue(destination, value, null);
            }
        }
    }
}
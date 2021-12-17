namespace Client.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contract;

    public class KnownCommandTypesAttribute : KnownTypesAttribute
    {
        public KnownCommandTypesAttribute() : base(new KnownTypesDataContractResolver(CommandTypes))
        {
        }

        private static IEnumerable<Type> CommandTypes =>
            from type in typeof(Contract.Commands.Orders.CreateOrder).Assembly.GetExportedTypes()
            where typeof(ICommand).IsAssignableFrom(type)
            where !type.IsAbstract
            select type;
    }
}
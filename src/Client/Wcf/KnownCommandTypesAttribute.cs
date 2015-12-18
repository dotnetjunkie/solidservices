namespace Client.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class KnownCommandTypesAttribute : KnownTypesAttribute
    {
        public KnownCommandTypesAttribute() : base(new KnownTypesDataContractResolver(CommandTypes))
        {
        }

        private static IEnumerable<Type> CommandTypes =>
            from type in typeof(Contract.Commands.Orders.CreateOrderCommand).Assembly.GetExportedTypes()
            where type.Name.EndsWith("Command")
            select type;
    }
}
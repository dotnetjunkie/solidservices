namespace BusinessLayer.CrossCuttingConcerns
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Logs information about the succesfull execution of a given TMessage, where the used template is specific to
    /// the TMessage type with its specified properties. For instance, it might log the following:
    /// <code>
    /// this.logger.LogInformation(
    ///     "{Message} executed in {Milliseconds} with parameters {OrderId}",
    ///     "GetOrderById", // <-- {Message}
    ///     stopwatch.ElapsedMilliseconds, // <-- {Milliseconds}
    ///     message.OrderId); // <-- {OrderId}
    /// </code>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class StructuredMessageLogger<TMessage>
    {
        private static readonly string MessageName;
        private static readonly PropertyInfo[] MessageProperties;
        private static readonly string LogTemplate;
        private static readonly Type[] SupportedPropertyTypes =
            typeof(int).Assembly.GetExportedTypes().Where(t => t.IsPrimitive)
            .Concat(new[] { typeof(string), typeof(Guid) })
            .ToArray();

        private readonly ILogger logger;

        static StructuredMessageLogger()
        {
            // PERF: By using a static constructor, initialization is done just once.
            MessageName = typeof(TMessage).Name;
            MessageProperties = GetLoggableMessageProperties();
            LogTemplate = "{Message} executed in {Milliseconds}";

            if (MessageProperties.Length > 0)
            {
                LogTemplate +=
                    " with parameters " +
                    string.Join(", ", MessageProperties.Select(prop => "{" + prop.Name + "}"));
            }
        }

        public StructuredMessageLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(TMessage message, TimeSpan elapsed)
        {
            object[] parameters = this.BuildParameters(message, elapsed);

            this.logger.LogInformation(LogTemplate, parameters);
        }

        private object[] BuildParameters(TMessage message, TimeSpan elapsed)
        {
            var parameters = new object[MessageProperties.Length + 2];

            parameters[0] = MessageName;
            parameters[1] = (long)elapsed.TotalMilliseconds;

            for (int i = 0; i < MessageProperties.Length; i++)
            {
                PropertyInfo property = MessageProperties[i];

                // PERF: PropertyInfo.GetValue is pretty slow. If needed this can be optimized by compiling Expression
                // trees.
                parameters[i + 2] = property.GetValue(message);
            }

            return parameters;
        }

        private static PropertyInfo[] GetLoggableMessageProperties()
        {
            // TODO: Filter out unwanted properties (e.g. complex one or one's with sensitive info). You
            // can do this based on an attribute that you place on the property or only include properties
            // of certain primitive types (or both). The example here uses a white list of supported types
            return (
                from prop in typeof(TMessage).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where SupportedPropertyTypes.Contains(prop.PropertyType)
                orderby prop.Name // Sorting is important, because ordering is not guaranteed across restarts
                select prop)
                .ToArray();
        }
    }
}
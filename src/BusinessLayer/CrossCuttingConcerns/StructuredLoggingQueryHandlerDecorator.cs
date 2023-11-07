namespace BusinessLayer.CrossCuttingConcerns
{
    using Contract;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public sealed class StructuredLoggingQueryHandlerDecorator<TQuery, TResult>
        : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private static readonly PropertyInfo[] QueryProperties;
        private static readonly string LogTemplate;
        private static readonly string QueryName;
        private static readonly Type[] SupportedPropertyTypes =
            typeof(int).Assembly.GetExportedTypes().Where(t => t.IsPrimitive)
            .Concat(new[] { typeof(string), typeof(Guid) })
            .ToArray();

        static StructuredLoggingQueryHandlerDecorator()
        {
            QueryName = typeof(TQuery).Name;
            QueryProperties = GetLoggableQueryProperties();

            LogTemplate = "Query {Query} executed in {Milliseconds}";

            if (QueryProperties.Length > 0)
            {
                LogTemplate +=
                    " with parameters " +
                    string.Join(", ", QueryProperties.Select(prop => "{" + prop.Name + "}"));
            }
        }

        private static PropertyInfo[] GetLoggableQueryProperties()
        {
            // TODO: Filter out unwanted properties (e.g. complex one or one's with sensitive info). You
            // can do this based on an attribute that you place on the property or only include properties
            // of certain primitive types (or both). The example here uses a white list of supported types
            return (
                from prop in typeof(TQuery).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where SupportedPropertyTypes.Contains(prop.PropertyType)
                orderby prop.Name // Sorting is important, because ordering is not guaranteed across restarts
                select prop)
                .ToArray();
        }

        private readonly ILogger logger;
        private readonly IQueryHandler<TQuery, TResult> decoratee;

        public StructuredLoggingQueryHandlerDecorator(ILogger logger, IQueryHandler<TQuery, TResult> decoratee)
        {
            this.logger = logger;
            this.decoratee = decoratee;
        }

        public TResult Handle(TQuery query)
        {
            var watch = Stopwatch.StartNew();

            var result = this.decoratee.Handle(query);

            this.LogQuery(query, watch.ElapsedMilliseconds);

            return result;
        }

        private void LogQuery(TQuery query, long elapsedMilliseconds)
        {
            object[] parameters = this.BuildParameters(query, elapsedMilliseconds);
            
            this.logger.LogInformation(LogTemplate, parameters);
        }

        private object[] BuildParameters(TQuery query, long elapsedMilliseconds)
        {
            var parameters = new object[QueryProperties.Length + 2];

            parameters[0] = QueryName;
            parameters[1] = elapsedMilliseconds;

            for (int i = 0; i < QueryProperties.Length; i++)
            {
                PropertyInfo property = QueryProperties[i];
                parameters[i + 2] = property.GetValue(query);
            }

            return parameters;
        }
    }
}
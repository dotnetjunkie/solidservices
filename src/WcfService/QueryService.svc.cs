namespace WcfService
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;
    using Code;

    [ServiceContract(Namespace = "http://www.cuttingedge.it/solid/queryservice/v1.0")]
    [ServiceKnownType(nameof(GetKnownTypes))]
    public class QueryService
    {
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider) =>
            Bootstrapper.GetQueryAndResultTypes();

        [OperationContract]
        [FaultContract(typeof(ValidationError))]
        public object Execute(dynamic query) => ExecuteQuery(query);

        internal static object ExecuteQuery(dynamic query)
        {
            Type queryType = query.GetType();

            dynamic queryHandler = Bootstrapper.GetQueryHandler(query.GetType());

            try
            {
                return queryHandler.Handle(query);
            }
            catch (Exception ex)
            {
                Bootstrapper.Log(ex);

                var faultException = WcfExceptionTranslator.CreateFaultExceptionOrNull(ex);

                if (faultException != null)
                {
                    throw faultException;
                }

                throw;
            }
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;

namespace WcfService.Code
{
    public static class WcfExceptionTranslator
    {
        public static FaultException CreateFaultExceptionOrNull(Exception exception)
        {
            if (exception is ValidationException)
            {
                return new FaultException<ValidationError>(
                    new ValidationError { ErrorMessage = exception.Message }, exception.Message);
            }

#if DEBUG
            return new FaultException(exception.ToString());
#else
            return null;
#endif
        }
    }
}
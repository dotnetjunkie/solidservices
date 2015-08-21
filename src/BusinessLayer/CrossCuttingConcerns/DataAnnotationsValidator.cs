namespace BusinessLayer.CrossCuttingConcerns
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DataAnnotationsValidator : IValidator
    {
        private readonly IServiceProvider container;

        public DataAnnotationsValidator(IServiceProvider container)
        {
            this.container = container;
        }

        void IValidator.ValidateObject(object instance)
        {
            var context = new ValidationContext(instance, this.container, null);

            // Throws an exception when instance is invalid.
            Validator.ValidateObject(instance, context);
        }
    }
}

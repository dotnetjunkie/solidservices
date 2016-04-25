namespace Contract.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NonEmptyGuidAttribute : ValidationAttribute
    {
        public NonEmptyGuidAttribute()
            : base("The {0} field is required.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!(value is Guid))
            {
                return false;
            }

            return ((Guid)value) != Guid.Empty;
        }
    }
}
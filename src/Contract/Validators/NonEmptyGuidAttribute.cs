namespace Contract.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NonEmptyGuidAttribute : RequiredAttribute
    {
        public NonEmptyGuidAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is Guid))
            {
                return false;
            }

            return ((Guid)value) != Guid.Empty;
        }
    }
}
namespace Contract.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NonEmptyGuidAttribute : RequiredAttribute
    {
        /// <inheritdoc />
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
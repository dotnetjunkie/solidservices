namespace BusinessLayer
{
    public interface IValidator
    {
        /// <summary>Validates the given instance.</summary>
        /// <param name="instance">The instance to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the instance is a null reference.</exception>
        /// <exception cref="ValidationException">Thrown when the instance is invalid.</exception>
        void ValidateObject(object instance);
    }
}
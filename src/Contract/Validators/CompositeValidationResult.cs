namespace Contract.Validators
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CompositeValidationResult : ValidationResult, IEnumerable<ValidationResult>
    {
        public CompositeValidationResult(string errorMessage, IEnumerable<ValidationResult> results) 
            : base(errorMessage)
        {
            this.Results = results;
        }

        public IEnumerable<ValidationResult> Results { get; }
        public IEnumerator<ValidationResult> GetEnumerator() => this.Results.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
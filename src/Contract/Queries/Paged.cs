namespace Contract.Queries
{
    using System.Runtime.Serialization;

    // Applying the DataContract attribute to generic types prevents WCF from postfixing the closed-generic 
    // type name with a seemingly random hexadecimal code.
    /// <summary>Contains a set of items for the requested page.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    [DataContract(Name = nameof(Paged<T>) + "Of{0}")]
    public class Paged<T>
    {
        /// <summary>Information about the requested page.</summary>
        [DataMember] public PageInfo Paging { get; set; }

        /// <summary>The list of items for the given page.</summary>
        [DataMember] public T[] Items { get; set; }
    }
}
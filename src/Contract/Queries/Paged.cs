namespace Contract.Queries
{
    using System.Runtime.Serialization;

    // Applying the DataContract attribute to generic types prevents WCF from postfixing the closed-generic 
    // type name with a seemingly random hexadecimal code.
    [DataContract(Name = nameof(Paged<T>) + "Of{0}")]
    public class Paged<T>
    {
        [DataMember] public PageInfo Paging { get; set; }

        [DataMember] public T[] Items { get; set; }
    }
}
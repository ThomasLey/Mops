namespace Mops.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StorageEndpointPrefixAttribute : Attribute
{
    public StorageEndpointPrefixAttribute(string prefix)
    {
        Prefix = prefix;
    }

    public string Prefix { get; set; }
}
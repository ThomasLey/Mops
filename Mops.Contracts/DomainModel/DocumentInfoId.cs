namespace Mops.Contracts.DomainModel;
public record DocumentInfoId(Guid Value)
{
    public static implicit operator DocumentInfoId(Guid value) => new(value);

    public DocumentInfoId() : this(Guid.NewGuid())
    {
    }
}
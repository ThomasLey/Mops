namespace Mops.Contracts.DomainModel;

public record FilemapId(Guid Value)
{
    public static implicit operator FilemapId(Guid value) => new(value);
    public FilemapId() : this(Guid.NewGuid())
    {
    }
}
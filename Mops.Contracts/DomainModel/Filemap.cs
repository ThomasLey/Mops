namespace Mops.Contracts.DomainModel;

public record Filemap(FilemapId Uuid, string Title, Filemap? Parent = null)
{
    public static Filemap Empty => new(new FilemapId(Guid.Empty), string.Empty);

    public static Filemap Create(FilemapId id, string title)
    {
        return new Filemap(id, title);
    }

    public static Filemap Create(string title)
    {
        return new Filemap(Guid.NewGuid(), title);
    }
}
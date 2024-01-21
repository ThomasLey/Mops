namespace Mops.Contracts.DomainModel;

public record DocumentInfo(DocumentInfoId Uuid, string Title, string Extension, Filemap Filemap)
{
    public string Hash { get; set; } = string.Empty;
    public DateOnly? CreationDate { get; set; }

    public List<string> Tags { get; set; } = new();

    public static DocumentInfo Create(Guid uuid, string title, string extension, Filemap filemap)
    {
        return new DocumentInfo(uuid, title, extension, filemap);
    }
    public static DocumentInfo Create(string title, string extension, Filemap filemap)
    {
        return new DocumentInfo(new DocumentInfoId(), title, extension, filemap);
    }
}
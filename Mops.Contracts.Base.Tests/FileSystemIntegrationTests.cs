using FluentAssertions;
using Mops.Contracts.DomainModel;
using NUnit.Framework;

namespace Mops.Contracts.Base.Tests;

[TestFixture]
[Explicit]
[Ignore("Integration tests")]
public class FileSystemIntegrationTests
{
    private const string TestDataRoot = "c:/Workspace/_mops/";
    private const string TestDataData = "c:/Workspace/_mops/data";

    private readonly Dictionary<string, Filemap> _filemaps = new()
    {
        { "ebooks", Filemap.Create(new FilemapId(Guid.Parse("2DA08FDA-A4F2-440B-ADEF-F7E0877901B2")), "ebooks") }
    };

    [Test]
    public async Task Test()
    {
        const string dstFolder = "Test01";
        var cs = "folder://" + TestDataRoot + "/" + dstFolder;
        if (Directory.Exists(Path.Combine(TestDataRoot, dstFolder)))
            Directory.Delete(Path.Combine(TestDataRoot, dstFolder), true);

        var dataFldr = new DirectoryInfo(TestDataData);

        var sut = new FileSystemStorageEndpoint(cs);

        if (!dataFldr.Exists) dataFldr.Create();
        var fileBytes = await File.ReadAllBytesAsync(Path.Combine(dataFldr.FullName, "ebook_go.pdf"));
        var doc = new DocumentInfo(Guid.NewGuid(), "Foo", "",_filemaps["ebooks"]);
        await sut.GetDocumentsAsync(CancellationToken.None);
        await sut.AddDocumentAsync(doc, null);

        var docs = await sut.GetDocumentsAsync(CancellationToken.None);

        docs.Should().NotBeNull();
    }

    [Test]
    public void ReadFileSystemDatabase()
    {
        var file = ".inventory.json";
        var outFile = ".files.json";
    }
}
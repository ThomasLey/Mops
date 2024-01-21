using FluentAssertions;
using Mops.Contracts.Base;
using Mops.Contracts.DomainModel;
using Mops.Contracts.Specs.Tests.Mocks;
using TechTalk.SpecFlow;

namespace Mops.Contracts.Specs.Tests;

[Binding]
public class StorageEndpointStepDefinitions
{
    private byte[] _content = null!;
    private DocumentInfo _document = null!;
    private int _offset;
    private IEnumerable<DocumentInfo> _result = Enumerable.Empty<DocumentInfo>();
    private DocumentInfo _returnedDocument = null!;
    private IStorageEndpoint _storage = null!;

    [Given(@"the '([^']*)' to a storage")]
    public void GivenTheConnectionStringToAStorage(string connectionString)
    {
        var storageFactory = new StorageEndpointFactory();
        storageFactory.Add(typeof(StorageEndpointStepDefinitions).Assembly);

        _storage = (StorageEndpointMock)storageFactory.GetStorage(connectionString);
    }

    [Given(@"an offset of (.*) is defined")]
    public void GivenAnOffsetIsDefined(int offset)
    {
        _offset = offset;
    }

    [When(@"a list of (.*) documents is requested")]
    public async void WhenAListOfDocumentsIsRequested(int amount)
    {
        _result = await _storage.GetDocumentsAsync(CancellationToken.None, amount, _offset);
    }

    [Then(@"the result should be (.*) documents")]
    public void ThenTheResultShouldBe(int expected)
    {
        _result.Count().Should().Be(expected);
    }

    [Given(@"a document with '([^']*)' exists")]
    public void GivenADocumentWithExists(string id)
    {
        var idSafe = new DocumentInfoId(Guid.Parse(id));
        _document = new DocumentInfo(idSafe, "Test", "", Filemap.Empty);
        ((StorageEndpointMock)_storage).EnsureDocumentInternal(_document);
    }

    [When(@"the document with '([^']*)' is requested")]
    public async Task WhenTheDocumentWithIsRequested(string id)
    {
        var idSafe = Guid.Parse(id);
        _returnedDocument = await _storage.GetDocumentAsync(idSafe);
    }

    [Then(@"the document is returned")]
    public void ThenTheDocumentIsReturned()
    {
        _returnedDocument.Uuid.Should().Be(_document.Uuid);
        _returnedDocument.Title.Should().Be(_document.Title);
    }

    [Given(@"a document with '([^']*)' is provided")]
    public void GivenADocumentWithIdIsProvided(string id)
    {
        _document = DocumentInfo.Create(Guid.Parse(id), "Test", "", Filemap.Empty);
    }

    [Given(@"a file is provided")]
    public void GivenAFileIsProvided()
    {
        _content = new byte[] { 123 };
    }

    [When(@"the document is added")]
    public async Task WhenTheDocumentIsAdded()
    {
        await _storage.AddDocumentAsync(_document, _content);
    }

    [Then(@"the document should have the property '([^']*)'")]
    public void ThenTheDocumentShouldHaveTheProperty(string propertyName)
    {
        _document.GetType()
            .GetProperties()
            .Should().Contain(x => x.Name == propertyName);
    }
}
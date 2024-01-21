using Mops.Contracts.Attributes;
using Mops.Contracts.DomainModel;
using Mops.Contracts.Exceptions;

#pragma warning disable IDE0028

namespace Mops.Contracts.Specs.Tests.Mocks;

[StorageEndpointPrefix("mock://")]
internal class StorageEndpointMock : IStorageEndpoint
{
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly Dictionary<string, string> _parameter = new();
    private readonly IList<DocumentInfo> _documents = new List<DocumentInfo>();

    public StorageEndpointMock(string connectionString)
    {
        foreach (var parameter in connectionString.Split(';'))
        {
            var parameterSplit = parameter.Trim().Split('=', StringSplitOptions.TrimEntries);
            if (parameterSplit.Length != 2 || string.IsNullOrWhiteSpace(parameterSplit[0]))
                throw new ArgumentException(
                    $"The parameter list of the mock storage must be a [key]=[value] semicolon separated list, but it is '{connectionString}'");

            _parameter.Add(parameterSplit[0].ToLower(), parameterSplit[1]);
        }
    }

    public Task InitStorageEndpoint()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DocumentInfo>> GetDocumentsAsync(CancellationToken token, int amount = 20,
        int offset = 0)
    {
        return await Task.FromResult(Enumerable
                .Range(1, amount)
                .Select(x => DocumentInfo.Create(x.ToString(), "",Filemap.Empty)))
            .ConfigureAwait(false);
    }

    public Task<DocumentInfo> GetDocumentAsync(Guid id)
    {
        var result = _documents.SingleOrDefault(x => x.Uuid.Value == id);
        if (result == null) throw new DocumentNotExistException(id);

        return Task.FromResult(result);
    }

    public Task AddDocumentAsync(DocumentInfo document, byte[] content)
    {
        _documents.Add(document);
        return Task.CompletedTask;
    }

    public Task<Filemap> HasFilemapAsync(Filemap query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Filemap>> GetFilemapsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Filemap> AddFilemap(Filemap filemap)
    {
        throw new NotImplementedException();
    }

    public void EnsureDocumentInternal(DocumentInfo documentInfo)
    {
        _documents.Add(documentInfo);
    }
}
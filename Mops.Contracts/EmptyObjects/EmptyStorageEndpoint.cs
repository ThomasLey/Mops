using Mops.Contracts.Attributes;
using Mops.Contracts.DomainModel;
using Mops.Contracts.Exceptions;

namespace Mops.Contracts.EmptyObjects;

[StorageEndpointPrefix("empty://")]
public class EmptyStorageEndpoint : IStorageEndpoint
{
    public async Task InitStorageEndpoint() { }

    public async Task<IEnumerable<DocumentInfo>> GetDocumentsAsync(CancellationToken token, int amount = 20,
        int offset = 0)
    {
        return await Task.FromResult(Enumerable.Empty<DocumentInfo>()).ConfigureAwait(false);
    }

    public Task<DocumentInfo> GetDocumentAsync(Guid id)
    {
        throw new DocumentNotExistException(id);
    }

    public Task AddDocumentAsync(DocumentInfo document, byte[] content)
    {
        return Task.CompletedTask;
    }

    public Task<Filemap> HasFilemapAsync(Filemap query)
    {
        return Task.FromResult(Filemap.Empty);
    }

    public Task<IEnumerable<Filemap>> GetFilemapsAsync()
    {
        return Task.FromResult(Enumerable.Empty<Filemap>());
    }

    public Task<Filemap> AddFilemap(Filemap filemap)
    {
        return Task.FromResult(Filemap.Empty);
    }
}
using Mops.Contracts.DomainModel;

namespace Mops.Contracts;

public interface IStorageEndpoint
{
    Task InitStorageEndpoint();

    //----------------------------------------------------------
    // document functions
    //----------------------------------------------------------
    Task<IEnumerable<DocumentInfo>> GetDocumentsAsync(CancellationToken token, int amount = 20, int offset = 0);
    Task<DocumentInfo> GetDocumentAsync(Guid id);
    Task AddDocumentAsync(DocumentInfo document, byte[] content);

    //----------------------------------------------------------
    // filemap functions
    //----------------------------------------------------------
    Task<Filemap> HasFilemapAsync(Filemap query);
    Task<IEnumerable<Filemap>> GetFilemapsAsync();
    Task<Filemap> AddFilemap(Filemap filemap);
}
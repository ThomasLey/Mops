using Microsoft.Extensions.Logging;
using Mops.Contracts;
using Mops.Contracts.Base;
using Mops.Contracts.Base.Internal;
using Mops.Contracts.Commands;
using Mops.Contracts.DomainModel;
using Mops.Contracts.Exceptions;

namespace Mops.Contracts.Base.Commands;

internal class AddFileCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IStorageEndpoint _storage;

    public AddFileCommand(ILogger logger, IStorageEndpoint storage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public bool CanExecute(string[] context)
    {
        return context.Length == 2
               && string.Compare(context[0], "addfile", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public async Task Execute(string[] context)
    {
        if (!CanExecute(context)) return;

        var file = new FileInfo(context[1]);
        if (!file.Exists) throw new FileNotFoundException("Provided file does not exist");

        // lets try to suggest a filemap depending on the source file folder or filename
        var suggest = new FilenameFilemapSuggest(() => _storage.GetFilemapsAsync().Result);
        var filemap = suggest.Suggest(file.DirectoryName ?? string.Empty);
        if (filemap.Uuid.Value == Guid.Empty)
            filemap = suggest.Suggest(file.Name);
        if (filemap.Uuid.Value == Guid.Empty)
            throw new EmptyGuidException();

        // lets use an internal helper to get information from the file
        var h = new PathGuidHelper();
        var document = h.TryParseFolderToDocument(file, filemap).Result;
        if (document.Uuid.Value == Guid.Empty) // it turns out, the document has no ID -yet
        {
            var dTmp = document;
            document = DocumentInfo.Create(dTmp.Title, dTmp.Extension, dTmp.Filemap);
            document.CreationDate = dTmp.CreationDate;
        }

        var fileContents = await File.ReadAllBytesAsync(file.FullName);
        await _storage.AddDocumentAsync(document, fileContents);
        file.Delete();
    }
}
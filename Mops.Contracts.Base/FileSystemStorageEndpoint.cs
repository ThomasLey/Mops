using System.Security.Cryptography;
using Mops.Contracts.Attributes;
using Mops.Contracts.Base.Internal;
using Mops.Contracts.DomainModel;
using Mops.Contracts.Exceptions;

namespace Mops.Contracts.Base;

/// <summary>
///     <see cref="IStorageEndpoint" /> which simply uses the file system and a json inventory file.
///     The inventory file is called ".inventory.json" and all files
/// </summary>
[StorageEndpointPrefix("file://")]
public class FileSystemStorageEndpoint : IStorageEndpoint
{
    private readonly string _connectionString;

    private readonly DirectoryInfo _folder;

    public FileSystemStorageEndpoint(string connectionString)
    {
        _connectionString = connectionString;
        var folder = _connectionString.Substring("file://".Length + 1);

        _folder = new DirectoryInfo(folder);
        _documentsLazy = new Lazy<IList<DocumentInfo>>(ReadInventory);
        _filemapsLazy = new Lazy<IList<Filemap>>(ReadFilemaps);
    }

    public async Task InitStorageEndpoint()
    {
        var h = new PathGuidHelper();
        var p = new PathPatternHelper();
        foreach (var dir in _folder.GetDirectories())
        {
            var filemap = await h.TryParseFolderToFilemap(dir);
            if (filemap.Uuid.Value == Guid.Empty)
                filemap = Filemap.Create(filemap.Title);

            // rename all files in folder
            await RenameFilesInFolder(dir, filemap);

            var newDirName = Path.Combine(_folder.FullName, p.SuggestFolder(filemap));

            if (string.Compare(dir.FullName, newDirName, StringComparison.InvariantCultureIgnoreCase) == 0)
                continue;

            Console.WriteLine($"Renaming folder for filemap {filemap.Title} ({filemap.Uuid.Value})");
            dir.MoveTo(newDirName);
        }

        return;
        // awesome inline function
        async Task RenameFilesInFolder(DirectoryInfo dir, Filemap filemap)
        {
            foreach (var file in dir.GetFiles())
            {
                var d = await h.TryParseFolderToDocument(file, filemap);
                if (d.Uuid.Value == Guid.Empty)
                {
                    var dTmp = d;
                    d = DocumentInfo.Create(dTmp.Title, dTmp.Extension, dTmp.Filemap);
                    d.CreationDate = dTmp.CreationDate;
                }

                var newFilename = Path.Combine(dir.FullName, p.SuggestFilename(d));

                if (string.Compare(file.FullName, newFilename, StringComparison.InvariantCultureIgnoreCase) == 0)
                    continue;

                Console.WriteLine($"Renaming file to {newFilename})");
                file.MoveTo(newFilename);
            }
        }

    }

    public Task<IEnumerable<DocumentInfo>> GetDocumentsAsync(CancellationToken token, int amount = 20, int offset = 0)
    {
        return Task.FromResult(_documentsLazy.Value.Skip(offset).Take(amount));
    }

    public Task<DocumentInfo> GetDocumentAsync(Guid id)
    {
        var result = _documentsLazy.Value.SingleOrDefault(x => x.Uuid.Value == id);
        if (result == null) throw new DocumentNotExistException(id);

        return Task.FromResult(result);
    }

    public Task AddDocumentAsync(DocumentInfo document, byte[] content)
    {
        var p = new PathPatternHelper();
        document.Hash = CalculateHash(content);

        var filemapDir = Path.Combine(_folder.FullName, p.SuggestFolder(document.Filemap));
        var dir = Directory.CreateDirectory(filemapDir);

        File.WriteAllBytesAsync(Path.Combine(dir.FullName, p.SuggestFilename(document)), content);
        // todo store hash
        _documentsLazy.Value.Add(document);

        return Task.CompletedTask;
    }

    public async Task<Filemap> HasFilemapAsync(Filemap query)
    {
        var result = _filemapsLazy.Value.SingleOrDefault(x => x.Uuid.Value == query.Uuid.Value)
                     ?? _filemapsLazy.Value.FirstOrDefault(x => x.Title.ToLower() == query.Title.ToLower())
                     ?? Filemap.Empty;

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Filemap>> GetFilemapsAsync()
    {
        return await Task.FromResult(_filemapsLazy.Value.ToArray());
    }

    public async Task<Filemap> AddFilemap(Filemap filemap)
    {
        if (filemap.Uuid.Value == Guid.Empty)
            throw new EmptyGuidException();

        var filemaps = await GetFilemapsAsync();
        var foundFilemap = filemaps.SingleOrDefault(x =>
            string.Compare(x.Title, filemap.Title, StringComparison.InvariantCultureIgnoreCase) == 0);
        if (foundFilemap != null) return foundFilemap;

        var p = new PathPatternHelper();
        var folderName = p.SuggestFolder(filemap);

        Directory.CreateDirectory(Path.Combine(_folder.FullName, folderName));
        return filemap;
    }

    public string CalculateHash(byte[] contents)
    {
        using var md5 = MD5.Create();

        return BitConverter.ToString(md5.ComputeHash(contents)).Replace("-", "");
    }

    #region lazy list of documents

    private readonly Lazy<IList<DocumentInfo>> _documentsLazy;

    private List<DocumentInfo> ReadInventory()
    {
        _folder.Create();
        var h = new PathGuidHelper();

        // todo: get all files from subfolders!
        var allFiles = _folder.GetDirectories()
            .SelectMany(x => x.GetFiles("*.*", SearchOption.TopDirectoryOnly));
        var inventory =
            allFiles.Select(x => h.TryParseFolderToDocument(x, h.TryParseFolderToFilemap(x.Directory).Result).Result);

        return inventory.ToList();
    }

    #endregion

    #region lazy list of filemaps

    private readonly Lazy<IList<Filemap>> _filemapsLazy;

    private List<Filemap> ReadFilemaps()
    {
        var h = new PathGuidHelper();
        var allDirectories = _folder
            .GetDirectories("*.*", SearchOption.AllDirectories);
        var inventory = allDirectories.Select(x => h.TryParseFolderToFilemap(x).Result);

        return inventory.ToList();
    }

    #endregion
}
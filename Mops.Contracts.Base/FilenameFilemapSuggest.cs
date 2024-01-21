using Mops.Contracts.DomainModel;

namespace Mops.Contracts.Base;

public class FilenameFilemapSuggest : ISuggest<Filemap>
{
    private readonly Func<IEnumerable<Filemap>> _filemapFactory;

    public FilenameFilemapSuggest(Func<IEnumerable<Filemap>> filemapFactory)
    {
        _filemapFactory = filemapFactory ?? throw new ArgumentNullException(nameof(filemapFactory));
    }

    public Filemap Suggest(string source)
    {
        var filemaps = _filemapFactory() ?? Enumerable.Empty<Filemap>();
        var lowerSource = source.ToLower();
        foreach (var filemap in filemaps)
            if (lowerSource.Contains(filemap.Title.ToLower()))
                return filemap;

        return Filemap.Empty;
    }
}
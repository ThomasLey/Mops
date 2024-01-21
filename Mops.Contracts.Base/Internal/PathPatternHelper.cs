using Mops.Contracts.DomainModel;

namespace Mops.Contracts.Base.Internal;

internal class PathPatternHelper
{
    public string SuggestFilename(DocumentInfo d)
    {
        if (d.CreationDate.HasValue)
        {
            return $"{d.CreationDate.Value.ToString("yyyyMMdd")}_{d.Title} ({d.Uuid.Value}){d.Extension}";
        }
        return $"{d.Title} ({d.Uuid.Value}){d.Extension}";
    }

    public string SuggestFolder(Filemap f)
    {
        return $"{f.Title} ({f.Uuid.Value})";
    }
}
using System.Text.RegularExpressions;
using Mops.Contracts.DomainModel;

namespace Mops.Contracts.Base.Internal;

public class PathGuidHelper
{
    /// <summary>
    /// Beware: The ID could be empty, in this case the ID cannot be read or does not exist
    /// </summary>
    internal Task<Filemap> TryParseFolderToFilemap(DirectoryInfo dir)
    {
        var id = GuidFromString(dir.Name);

        var title = dir.Name
            .Replace("(" + id.ToString().ToLower() + ")", "")
            .Replace("(" + id.ToString().ToUpper() + ")", "")
            .Replace(id.ToString().ToLower(), "")
            .Replace(id.ToString().ToUpper(), "")
            .Trim('_').Trim('-').Trim();

        return Task.FromResult(Filemap.Create(id, title));
    }

    /// <suMMary>
    /// Beware: The ID could be empty, in this case the ID cannot be read or does not exist
    /// </suMMary>
    public async Task<DocumentInfo> TryParseFolderToDocument(FileInfo file, Filemap filemap)
    {
        var title = Path.GetFileNameWithoutExtension(file.Name);
        var id = GuidFromString(file.Name);
        var creationDate = DateOnlyFromString(file.Name);
        if (creationDate.HasValue)
        {
            title = title.Replace(creationDate.Value.ToString("yyyy-MM-dd"), "")
                .Replace(creationDate.Value.ToString("yyyy_MM_dd"), "")
                .Replace(creationDate.Value.ToString("yyyyMMdd"), "");
        }


        title = title.Replace("(" + id.ToString().ToLower() + ")", "")
            .Replace("(" + id.ToString().ToUpper() + ")", "")
            .Replace(id.ToString().ToLower(), "")
            .Replace(id.ToString().ToUpper(), "")
            .Trim('_').Trim('-').Trim();


        var result = DocumentInfo.Create(id, title, file.Extension, filemap);
        result.CreationDate = creationDate;
        return result;
    }

    public static Guid GuidFromString(string s)
    {
        var ids = Regex.Matches(s, @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}");
        if (ids.Count == 0) return Guid.Empty;
        return Guid.Parse(ids[0].Value);
    }

    public static DateOnly? DateOnlyFromString(string s)
    {
        if (DateOnly.TryParseExact(s[0..10], "yyyy-MM-dd", out DateOnly dateOnly1)) return dateOnly1;
        if (DateOnly.TryParseExact(s[0..10], "yyyy_MM_dd", out DateOnly dateOnly2)) return dateOnly2;
        if (DateOnly.TryParseExact(s[0..8], "yyyyMMdd", out DateOnly dateOnly3)) return dateOnly3;
        return null;
    }
}
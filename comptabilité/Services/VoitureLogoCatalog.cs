using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace comptabilité.Services;

// Associe une voiture à son logo vectoriel (wwwroot/images/logos/{nom}.svg) d'après son nom.
// "Mustang GT" retombe sur "mustang" si "mustang-gt.svg" n'existe pas.
public class VoitureLogoCatalog
{
    private const string Folder = "images/logos";

    private readonly IFileProvider _webRoot;
    private readonly ConcurrentDictionary<string, string?> _cache = new();

    public VoitureLogoCatalog(IWebHostEnvironment environment)
    {
        _webRoot = environment.WebRootFileProvider;
    }

    public string? GetPath(string? nom)
    {
        var slug = Slugify(nom);
        if (slug.Length == 0)
        {
            return null;
        }

        return _cache.GetOrAdd(slug, Resolve);
    }

    private string? Resolve(string slug)
    {
        var candidates = new List<string> { slug };
        var firstWord = slug.Split('-')[0];
        if (firstWord != slug)
        {
            candidates.Add(firstWord);
        }

        foreach (var candidate in candidates)
        {
            var path = $"{Folder}/{candidate}.svg";
            if (_webRoot.GetFileInfo(path).Exists)
            {
                return path;
            }
        }

        return null;
    }

    private static string Slugify(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var c in value.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lower = char.ToLowerInvariant(c);
            if (lower is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                builder.Append(lower);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }
}

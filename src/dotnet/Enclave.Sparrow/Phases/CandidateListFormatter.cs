using Enclave.Echelon.Core.Models;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Formats the candidate list as multi-column, alphabetical (SPARROW-Requirements: columns derived from word length).
/// </summary>
internal static class CandidateListFormatter
{
    private static readonly StringComparer _caseInsensitive = StringComparer.OrdinalIgnoreCase;

    /// <summary>Formats candidates in columns (column count = word length), row-major, alphabetically by Word.</summary>
    public static string Format(IEnumerable<Password> candidates, int wordLength)
    {
        var sorted = candidates.OrderBy(c => c.Word, _caseInsensitive).ToList();
        if (sorted.Count == 0) return string.Empty;
        var columns = Math.Max(1, wordLength);
        var colWidth = wordLength + 2;
        var sb = new System.Text.StringBuilder();

        for (var i = 0; i < sorted.Count; i++)
        {
            if (i > 0 && i % columns == 0)
                sb.AppendLine();
            else if (i > 0)
                sb.Append(' ', colWidth - sorted[i - 1].Word.Length);
            sb.Append(sorted[i].Word);
        }

        return sb.ToString();
    }
}

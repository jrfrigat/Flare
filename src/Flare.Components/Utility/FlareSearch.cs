namespace Flare.Components;

/// <summary>
/// Relevance scoring for type-ahead search. Components such as <c>FlareCombobox</c> and
/// <c>FlareMultiSelect</c> use <see cref="Score"/> to rank matches (best first) instead of a plain
/// alphabetical or insertion-order filter, so typing "lo" surfaces "London" above "Los Angeles".
/// Consumers can also call it directly to build a custom <c>RankFunc</c>.
/// </summary>
public static class FlareSearch
{
    /// <summary>
    /// Scores how well <paramref name="text"/> matches <paramref name="query"/> (case-insensitive):
    /// 0 means no match, larger is more relevant. The bands are exact &gt; prefix &gt; word-start &gt;
    /// substring (earlier is better) &gt; subsequence (fuzzy). An empty query matches everything.
    /// </summary>
    public static double Score(string? text, string? query)
    {
        if (string.IsNullOrEmpty(query)) return 1;
        if (string.IsNullOrEmpty(text)) return 0;

        var t = text.ToLowerInvariant();
        var q = query.ToLowerInvariant();

        if (t == q) return 1000;
        if (t.StartsWith(q, StringComparison.Ordinal)) return 900 - Pen(t.Length);

        var wb = WordStartIndex(t, q);
        if (wb >= 0) return 700 - Pen(wb);

        var idx = t.IndexOf(q, StringComparison.Ordinal);
        if (idx >= 0) return 500 - Pen(idx);

        if (IsSubsequence(t, q)) return 300 - Pen(t.Length - q.Length);

        return 0;
    }

    /// <summary>
    /// Filters and orders <paramref name="items"/> by a relevance <paramref name="score"/>, keeping
    /// only positive scores and ordering best-first (ties keep the original order, stable).
    /// </summary>
    public static IEnumerable<T> Rank<T>(IEnumerable<T> items, Func<T, double> score) =>
        items.Select((item, i) => (item, i, s: score(item)))
             .Where(x => x.s > 0)
             .OrderByDescending(x => x.s)
             .ThenBy(x => x.i)
             .Select(x => x.item);

    // A small in-band penalty (max ~49.5) so longer/later matches rank below shorter/earlier ones
    // without ever crossing into the next-lower band or dropping to zero.
    private static double Pen(int n) => Math.Min(99, Math.Max(0, n)) * 0.5;

    // First occurrence of `query` that begins a word (string start or just after a separator).
    private static int WordStartIndex(string text, string query)
    {
        var i = 0;
        while ((i = text.IndexOf(query, i, StringComparison.Ordinal)) >= 0)
        {
            if (i == 0 || text[i - 1] is ' ' or '-' or '_' or '/' or '.' or ',') return i;
            i++;
        }
        return -1;
    }

    // Whether every character of `query` appears in `text` in order (a fuzzy match).
    private static bool IsSubsequence(string text, string query)
    {
        var qi = 0;
        foreach (var c in text)
        {
            if (qi < query.Length && c == query[qi]) qi++;
            if (qi == query.Length) return true;
        }
        return qi == query.Length;
    }
}

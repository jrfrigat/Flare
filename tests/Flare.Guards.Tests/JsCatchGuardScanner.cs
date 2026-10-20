namespace Flare.Guards.Tests;

/// <summary>
/// Finds a catch of <c>JSDisconnectedException</c> that is not followed by a catch of
/// <c>JSException</c> in the same try. The source is parsed as code rather than matched as text:
/// comments and literals are blanked first, so a commented-out catch cannot satisfy the rule, and
/// the clause body is brace-matched, so a nested try/catch does not count as its neighbour.
/// </summary>
internal static class JsCatchGuardScanner
{
    private const string Disconnected = "JSDisconnectedException";
    private const string Js = "JSException";

    /// <summary>The 1-based line numbers of the disconnected catches that are not protected.</summary>
    public static IReadOnlyList<int> FindOffenders(string source)
    {
        var code = StripCommentsAndLiterals(source);
        var offenders = new List<int>();
        var from = 0;
        while (true)
        {
            var at = FindKeyword(code, "catch", from);
            if (at < 0) break;
            from = at + 5;
            var clause = ParseCatch(code, at);
            if (clause is null || clause.Type != Disconnected) continue;
            // Only whitespace may stand between two catch clauses of the same try; anything else
            // means this one is the last, or a finally follows.
            var next = clause.End;
            SkipWhitespace(code, ref next);
            if (!IsKeywordAt(code, "catch", next) || ParseCatch(code, next) is not { Type: Js })
                offenders.Add(LineAt(code, at));
        }
        return offenders;
    }

    /// <summary>
    /// Replaces comments and string or character literals with spaces, keeping every newline so a
    /// line number taken from the result points at the original line.
    /// </summary>
    public static string StripCommentsAndLiterals(string source)
    {
        var text = source.ToCharArray();
        var i = 0;
        while (i < text.Length)
        {
            var c = text[i];
            if (c == '/' && i + 1 < text.Length && text[i + 1] == '/')
            {
                while (i < text.Length && text[i] != '\n') Blank(text, i++);
                continue;
            }
            if (c == '/' && i + 1 < text.Length && text[i + 1] == '*')
            {
                Blank(text, i++);
                Blank(text, i++);
                while (i < text.Length && !(text[i] == '*' && i + 1 < text.Length && text[i + 1] == '/'))
                    Blank(text, i++);
                if (i < text.Length) Blank(text, i++);
                if (i < text.Length) Blank(text, i++);
                continue;
            }
            if (c == '\'' && IsCharLiteral(text, i))
            {
                i = BlankQuoted(text, i, '\'');
                continue;
            }
            if (c == '"' || c == '@' || c == '$')
            {
                if (TryStringStart(text, i, out var contentAt, out var verbatim, out var rawQuotes))
                {
                    i = BlankString(text, i, contentAt, verbatim, rawQuotes);
                    continue;
                }
            }
            i++;
        }
        return new string(text);
    }

    // A string starts at i with "..." (regular), @"..." (verbatim), $"..." (interpolated) or a
    // run of dollars and quotes for a raw string. contentAt is the first character after the
    // opening delimiter; rawQuotes is the delimiter length for a raw string, zero otherwise.
    private static bool TryStringStart(char[] text, int i, out int contentAt, out bool verbatim, out int rawQuotes)
    {
        contentAt = i;
        verbatim = false;
        rawQuotes = 0;
        var at = i;
        while (at < text.Length && text[at] == '$') at++;
        var dollars = at - i;
        if (at < text.Length && text[at] == '@')
        {
            if (dollars > 1) return false;
            verbatim = true;
            at++;
            if (at >= text.Length || text[at] != '"') return false;
        }
        if (at < text.Length && text[at] == '@' && dollars == 0)
        {
            verbatim = true;
            at++;
        }
        if (at >= text.Length || text[at] != '"') return false;
        var quotes = 0;
        while (at + quotes < text.Length && text[at + quotes] == '"') quotes++;
        if (quotes >= 3)
        {
            rawQuotes = quotes;
            contentAt = at + quotes;
            return true;
        }
        if (quotes == 2 && dollars == 0)
        {
            // Two quotes are an empty regular string only when they close right away; otherwise
            // the second quote opens the string and the first is not a delimiter.
            contentAt = i + 1;
            return true;
        }
        contentAt = at + 1;
        return true;
    }

    private static int BlankString(char[] text, int start, int contentAt, bool verbatim, int rawQuotes)
    {
        for (var opening = start; opening < contentAt && opening < text.Length; opening++) Blank(text, opening);
        var i = contentAt;
        if (rawQuotes > 0)
        {
            while (i < text.Length)
            {
                var run = 0;
                while (i + run < text.Length && text[i + run] == '"') run++;
                if (run >= rawQuotes)
                {
                    for (var k = 0; k < run; k++) Blank(text, i + k);
                    return i + run;
                }
                if (text[i] != '\n' && text[i] != '\r') Blank(text, i);
                i++;
            }
            return i;
        }
        while (i < text.Length)
        {
            var c = text[i];
            if (c == '"')
            {
                if (verbatim && i + 1 < text.Length && text[i + 1] == '"')
                {
                    Blank(text, i++);
                    Blank(text, i++);
                    continue;
                }
                Blank(text, i++);
                return i;
            }
            if (!verbatim && c == '\\' && i + 1 < text.Length)
            {
                Blank(text, i++);
                if (i < text.Length && text[i] != '\n') Blank(text, i++);
                continue;
            }
            if (c != '\n' && c != '\r') Blank(text, i);
            i++;
        }
        return i;
    }

    private static int BlankQuoted(char[] text, int start, char quote)
    {
        Blank(text, start);
        var i = start + 1;
        while (i < text.Length)
        {
            if (text[i] == '\\' && i + 1 < text.Length)
            {
                Blank(text, i++);
                Blank(text, i++);
                continue;
            }
            if (text[i] == quote)
            {
                Blank(text, i++);
                return i;
            }
            if (text[i] != '\n' && text[i] != '\r') Blank(text, i);
            i++;
        }
        return i;
    }

    private static void Blank(char[] text, int at)
    {
        if (text[at] != '\n' && text[at] != '\r') text[at] = ' ';
    }

    private static int FindKeyword(string code, string keyword, int from)
    {
        for (var i = from; i + keyword.Length <= code.Length; i++)
        {
            if (IsKeywordAt(code, keyword, i)) return i;
        }
        return -1;
    }

    private static bool IsKeywordAt(string code, string keyword, int at)
        => at + keyword.Length <= code.Length
           && string.CompareOrdinal(code, at, keyword, 0, keyword.Length) == 0
           && (at == 0 || !IsIdentifier(code[at - 1]))
           && (at + keyword.Length == code.Length || !IsIdentifier(code[at + keyword.Length]));

    // An apostrophe in markup text is not a char literal: only a closing quote within a short
    // escape-free span counts, so Razor prose cannot blank the code after it.
    private static bool IsCharLiteral(char[] text, int at)
    {
        if (at + 2 < text.Length && text[at + 1] != '\\' && text[at + 2] == '\'') return true;
        if (at + 1 >= text.Length || text[at + 1] != '\\') return false;
        for (var i = at + 2; i < text.Length && i < at + 12 && text[i] != '\n'; i++)
        {
            if (text[i] == '\'') return true;
        }
        return false;
    }

    private static bool IsIdentifier(char c) => char.IsAsciiLetterOrDigit(c) || c == '_';

    private static int LineAt(string code, int at)
    {
        var line = 1;
        for (var i = 0; i < at && i < code.Length; i++)
            if (code[i] == '\n') line++;
        return line;
    }

    private sealed record CatchClause(string Type, int End);

    // Parses `catch (Type)` / `catch (Type name)` / `catch (Type name) when (...)` and its body.
    // End is the position after the body's closing brace.
    private static CatchClause? ParseCatch(string code, int catchAt)
    {
        var i = catchAt + 5;
        SkipWhitespace(code, ref i);
        if (i >= code.Length || code[i] != '(') return null;
        var open = i;
        var close = MatchPair(code, open, '(', ')');
        if (close < 0) return null;
        var header = code[(open + 1)..close].Trim();
        if (header.Length == 0) return null;
        var type = header.Split([' ', '\t', '\r', '\n', ','], StringSplitOptions.RemoveEmptyEntries)[0];
        type = type[(type.LastIndexOf('.') + 1)..];
        i = close + 1;
        SkipWhitespace(code, ref i);
        if (string.CompareOrdinal(code, i, "when", 0, 4) == 0 && !IsIdentifier(i + 4 < code.Length ? code[i + 4] : ' '))
        {
            i += 4;
            SkipWhitespace(code, ref i);
            if (i < code.Length && code[i] == '(')
            {
                var filterEnd = MatchPair(code, i, '(', ')');
                if (filterEnd < 0) return null;
                i = filterEnd + 1;
            }
        }
        while (i < code.Length && char.IsWhiteSpace(code[i])) i++;
        if (i >= code.Length || code[i] != '{') return null;
        var bodyEnd = MatchPair(code, i, '{', '}');
        if (bodyEnd < 0) return null;
        return new CatchClause(type, bodyEnd + 1);
    }

    private static int MatchPair(string code, int openAt, char open, char close)
    {
        var depth = 0;
        for (var i = openAt; i < code.Length; i++)
        {
            if (code[i] == open) depth++;
            else if (code[i] == close && --depth == 0) return i;
        }
        return -1;
    }

    private static void SkipWhitespace(string code, ref int i)
    {
        while (i < code.Length && char.IsWhiteSpace(code[i])) i++;
    }
}

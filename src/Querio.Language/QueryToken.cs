using System.Collections.Generic;
using System.Text;

namespace Querio.Language;

/// <summary>What a piece of query text is.</summary>
public enum QueryTokenKind
{
    /// <summary>A bare word: a keyword, or a name that needed no brackets.</summary>
    Word,

    /// <summary>A name written in brackets, which is never read as a keyword.</summary>
    Quoted,

    /// <summary>A text value in single quotes.</summary>
    Text,

    /// <summary>A number.</summary>
    Number,

    /// <summary>Anything else: a bracket, a comma, a dot, an operator.</summary>
    Symbol,

    /// <summary>The end of the text.</summary>
    End,
}

/// <summary>
/// One piece of query text, with where it came from. Positions are kept because everything built on
/// top of this needs them: an editor underlines a problem, and completion has to know which token
/// the caret is sitting in.
/// </summary>
/// <param name="Kind">What the piece is.</param>
/// <param name="Text">Its value, with brackets or quotes already taken off.</param>
/// <param name="Start">Zero-based offset of the first character, brackets included.</param>
/// <param name="Length">Length in characters, brackets included.</param>
public readonly record struct QueryToken(QueryTokenKind Kind, string Text, int Start, int Length)
{
    /// <summary>One past the last character.</summary>
    public int End => Start + Length;

    /// <summary>Whether this is a bare word matching the given keyword, ignoring case.</summary>
    /// <param name="keyword">The keyword to test for.</param>
    public bool Is(string keyword)
        => Kind == QueryTokenKind.Word && string.Equals(Text, keyword, StringComparison.OrdinalIgnoreCase);

    /// <summary>Whether this is the given symbol.</summary>
    /// <param name="symbol">The symbol to test for.</param>
    public bool IsSymbol(string symbol)
        => Kind == QueryTokenKind.Symbol && string.Equals(Text, symbol, StringComparison.Ordinal);

    /// <summary>Whether this token can stand for a name.</summary>
    public bool IsName => Kind is QueryTokenKind.Word or QueryTokenKind.Quoted;

    /// <summary>Whether the caret at this offset is inside or immediately after the token.</summary>
    /// <param name="caret">Zero-based caret offset.</param>
    public bool Touches(int caret) => caret >= Start && caret <= End;
}

/// <summary>
/// Splits query text into pieces.
/// <para>
/// Deliberately forgiving: text being typed is unfinished almost all of the time, so an unclosed
/// bracket or quote ends at the end of the text rather than failing. Deciding what is wrong is the
/// parser's job, and it can only do that once it can see the pieces.
/// </para>
/// </summary>
public static class QueryLexer
{
    private const string Symbols = "().,*=<>!+-/%";

    /// <summary>Splits text into tokens, ending with a single <see cref="QueryTokenKind.End"/>.</summary>
    /// <param name="text">The query text.</param>
    public static IReadOnlyList<QueryToken> Split(string text)
    {
        var tokens = new List<QueryToken>();
        if (text is null) text = string.Empty;

        var at = 0;
        while (at < text.Length)
        {
            if (char.IsWhiteSpace(text[at])) { at++; continue; }

            var start = at;
            var character = text[at];

            if (character == '[')
            {
                at++;
                var value = new StringBuilder();
                while (at < text.Length)
                {
                    if (text[at] == ']')
                    {
                        // Two closing brackets in a row are one bracket in the name, as in T-SQL.
                        if (at + 1 < text.Length && text[at + 1] == ']') { value.Append(']'); at += 2; continue; }
                        at++;
                        break;
                    }
                    value.Append(text[at++]);
                }
                tokens.Add(new QueryToken(QueryTokenKind.Quoted, value.ToString(), start, at - start));
                continue;
            }

            if (character == '\'')
            {
                at++;
                var value = new StringBuilder();
                while (at < text.Length)
                {
                    if (text[at] == '\'')
                    {
                        if (at + 1 < text.Length && text[at + 1] == '\'') { value.Append('\''); at += 2; continue; }
                        at++;
                        break;
                    }
                    value.Append(text[at++]);
                }
                tokens.Add(new QueryToken(QueryTokenKind.Text, value.ToString(), start, at - start));
                continue;
            }

            if (char.IsDigit(character))
            {
                while (at < text.Length && (char.IsDigit(text[at]) || text[at] == '.')) at++;
                tokens.Add(new QueryToken(
                    QueryTokenKind.Number, text.Substring(start, at - start), start, at - start));
                continue;
            }

            if (char.IsLetter(character) || character == '_' || character == '@')
            {
                while (at < text.Length && (char.IsLetterOrDigit(text[at]) || text[at] == '_' || text[at] == '@')) at++;
                tokens.Add(new QueryToken(
                    QueryTokenKind.Word, text.Substring(start, at - start), start, at - start));
                continue;
            }

            // Two-character comparisons have to be taken whole, or ">=" reads as ">" then "=".
            if (at + 1 < text.Length && (text.Substring(at, 2) is ">=" or "<=" or "<>" or "!="))
            {
                tokens.Add(new QueryToken(QueryTokenKind.Symbol, text.Substring(at, 2), start, 2));
                at += 2;
                continue;
            }

            if (Symbols.IndexOf(character) >= 0)
            {
                tokens.Add(new QueryToken(QueryTokenKind.Symbol, character.ToString(), start, 1));
                at++;
                continue;
            }

            // Anything unrecognised is still a token, so the parser can point at it.
            tokens.Add(new QueryToken(QueryTokenKind.Symbol, character.ToString(), start, 1));
            at++;
        }

        tokens.Add(new QueryToken(QueryTokenKind.End, string.Empty, text.Length, 0));
        return tokens;
    }
}

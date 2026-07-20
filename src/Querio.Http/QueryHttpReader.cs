using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.Http;

/// <summary>
/// Reads a query back out of a query string.
/// <para>
/// The awkward half of going both ways. Writing can always succeed; reading cannot, because text is
/// free to say things this model has no room for. Everything here either produces exactly what was
/// written or refuses and says where - it never salvages what it half understood.
/// </para>
/// </summary>
internal sealed class QueryHttpReader
{
    private static readonly string[] Aggregates =
        ["count", "sum", "avg", "min", "max", "percentile"];

    private readonly string _text;
    private int _at;

    private QueryHttpReader(string text)
    {
        _text = text;
        _at = 0;
    }

    internal static QuerySpec Read(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new QueryParseException("There is nothing to read.");

        var parameters = new List<KeyValuePair<string, string>>();
        foreach (var pair in query.TrimStart('?').Split('&'))
        {
            if (pair.Length == 0) continue;
            var split = pair.IndexOf('=');
            parameters.Add(split < 0
                ? new KeyValuePair<string, string>(pair.Trim(), string.Empty)
                : new KeyValuePair<string, string>(
                    pair.Substring(0, split).Trim(), pair.Substring(split + 1).Trim()));
        }

        string? One(string name) => parameters
            .Where(p => string.Equals(p.Key, name, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Value)
            .FirstOrDefault();

        var from = One("from") ?? throw new QueryParseException("A query has to say what it draws from ('from').");
        var spec = new QuerySpec(ReadSource(from))
        {
            Joins = parameters
                .Where(p => string.Equals(p.Key, "join", StringComparison.OrdinalIgnoreCase))
                .Select(p => ReadJoin(p.Value))
                .ToList(),
            Select = Each(One("select")).Select(ReadSelect).ToList(),
            Where = ReadFilter(One("where")),
            GroupBy = Each(One("groupby")).Select(ReadGroupBy).ToList(),
            Having = ReadFilter(One("having")),
            OrderBy = Each(One("orderby")).Select(ReadSort).ToList(),
            Distinct = string.Equals(One("distinct"), "true", StringComparison.OrdinalIgnoreCase),
            Limit = ReadCount(One("top"), "top"),
            Offset = ReadCount(One("skip"), "skip"),
        };
        return spec;
    }

    private static IEnumerable<string> Each(string? list)
        => string.IsNullOrWhiteSpace(list)
            ? []
            : Split(list!, ',').Select(part => part.Trim()).Where(part => part.Length > 0);

    private static int? ReadCount(string? raw, string name)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            throw new QueryParseException($"'{name}' has to be a whole number, and '{raw}' is not.");
        }
        return value;
    }

    // ---- Sources and joins -----------------------------------------------------------------------

    private static QuerySource ReadSource(string text)
    {
        var fields = Split(text, ':');
        if (fields.Count != 2)
        {
            throw new QueryParseException($"A source reads as 'entity:alias', and '{text}' does not.");
        }

        var alias = fields[1].Trim();
        var body = fields[0].Trim();
        return body.EndsWith(")", StringComparison.Ordinal)
            ? QuerySource.FromFunction(new QueryHttpReader(body).ReadCall(), alias)
            : new QuerySource(body, alias);
    }

    private static QueryJoin ReadJoin(string text)
    {
        var fields = Split(text, ':').Select(part => part.Trim()).ToList();
        if (fields.Count < 2)
        {
            throw new QueryParseException(
                $"A join reads as 'entity:alias[:relation|on(...)][:kind][:from]', and '{text}' does not.");
        }

        var body = fields[0];
        var call = body.EndsWith(")", StringComparison.Ordinal) && !body.StartsWith("on(", StringComparison.Ordinal)
            ? new QueryHttpReader(body).ReadCall()
            : null;

        var join = new QueryJoin(call is null ? body : null, fields[1]) { Call = call };

        var match = fields.Count > 2 ? fields[2] : string.Empty;
        if (match.StartsWith("on(", StringComparison.Ordinal))
        {
            join = join with { On = ReadJoinConditions(match) };
        }
        else if (match.Length > 0)
        {
            join = join with { Relation = match };
        }

        if (fields.Count > 3 && fields[3].Length > 0)
        {
            join = join with { Kind = ReadEnum<QueryJoinKind>(fields[3], "join kind") };
        }
        if (fields.Count > 4 && fields[4].Length > 0) join = join with { From = fields[4] };
        return join;
    }

    private static IReadOnlyList<QueryJoinCondition> ReadJoinConditions(string text)
    {
        var inner = text.Substring(3, text.Length - 4);
        return Split(inner, ',').Select(pair =>
        {
            var sides = pair.Split('=');
            if (sides.Length != 2)
            {
                throw new QueryParseException($"A join match reads as 'a.x=b.y', and '{pair}' does not.");
            }
            return new QueryJoinCondition(ReadFieldRef(sides[0].Trim()), ReadFieldRef(sides[1].Trim()));
        }).ToList();
    }

    // ---- Selected items, grouping and ordering ---------------------------------------------------

    private static QuerySelect ReadSelect(string text)
    {
        var (body, alias) = SplitAlias(text);
        var name = LeadingCall(body);

        if (name is not null && Aggregates.Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            return ReadAggregate(name, body, alias);
        }

        var (value, period) = SplitPeriod(body);
        var (field, call) = ReadValue(value);
        return new QuerySelect { Field = field, Call = call, Truncate = period, Alias = alias };
    }

    private static QuerySelect ReadAggregate(string name, string body, string? alias)
    {
        var aggregate = ReadEnum<QueryAggregate>(name, "aggregate");
        var inner = Inside(body).Trim();
        var distinct = inner.StartsWith("distinct ", StringComparison.OrdinalIgnoreCase);
        if (distinct) inner = inner.Substring("distinct ".Length).Trim();

        double? rank = null;
        if (aggregate == QueryAggregate.Percentile)
        {
            var arguments = Split(inner, ',');
            if (arguments.Count != 2)
            {
                throw new QueryParseException($"A percentile reads as 'percentile(value, rank)', and '{body}' does not.");
            }
            inner = arguments[0].Trim();
            if (!double.TryParse(arguments[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                throw new QueryParseException($"A percentile rank has to be a number, and '{arguments[1]}' is not.");
            }
            rank = parsed;
        }

        var (field, call) = inner.Length == 0 ? (null, null) : ReadValue(inner);
        return new QuerySelect
        {
            Field = field,
            Call = call,
            Aggregate = aggregate,
            Distinct = distinct,
            Percentile = rank,
            Alias = alias,
        };
    }

    private static QueryGroupBy ReadGroupBy(string text)
    {
        var (body, alias) = SplitAlias(text);
        var (value, period) = SplitPeriod(body);
        var (field, call) = ReadValue(value);
        return new QueryGroupBy(field) { Call = call, Truncate = period, Alias = alias };
    }

    private static QuerySort ReadSort(string text)
    {
        var body = text.Trim();
        var direction = QuerySortDirection.Ascending;
        foreach (var suffix in new[] { " desc", " asc" })
        {
            if (!body.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;
            direction = suffix == " desc" ? QuerySortDirection.Descending : QuerySortDirection.Ascending;
            body = body.Substring(0, body.Length - suffix.Length).Trim();
            break;
        }

        // A bare name is an output name; anything qualified or called is a value.
        if (body.IndexOf('.') < 0 && body.IndexOf('(') < 0)
        {
            return new QuerySort { Select = body, Direction = direction };
        }
        var (field, call) = ReadValue(body);
        return new QuerySort { Field = field, Call = call, Direction = direction };
    }

    // ---- Conditions ------------------------------------------------------------------------------

    private static QueryFilterGroup? ReadFilter(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var reader = new QueryHttpReader(text!);
        var group = reader.ParseOr();
        reader.SkipSpace();
        if (reader._at < reader._text.Length)
        {
            throw new QueryParseException($"'{reader._text.Substring(reader._at)}' was not expected", reader._at);
        }
        return group;
    }

    private QueryFilterGroup ParseOr() => ParseChain(or: true);

    private QueryFilterGroup ParseChain(bool or)
    {
        var parts = new List<QueryFilterGroup> { or ? ParseChain(false) : ParseFactor() };
        while (TryWord(or ? "or" : "and")) parts.Add(or ? ParseChain(false) : ParseFactor());
        if (parts.Count == 1) return parts[0];

        // A part holding one condition and nothing else folds in, so the tree stays as shallow as
        // the text was.
        var group = new QueryFilterGroup { Or = or };
        var conditions = new List<QueryCondition>();
        var groups = new List<QueryFilterGroup>();
        foreach (var part in parts)
        {
            if (part.Conditions.Count == 1 && part.Groups.Count == 0) conditions.Add(part.Conditions[0]);
            else groups.Add(part);
        }
        return group with { Conditions = conditions, Groups = groups };
    }

    private QueryFilterGroup ParseFactor()
    {
        SkipSpace();
        if (Peek() == '(')
        {
            _at++;
            var inner = ParseOr();
            SkipSpace();
            if (Peek() != ')') throw new QueryParseException("A bracket was opened and never closed", _at);
            _at++;
            return inner;
        }
        return new QueryFilterGroup { Conditions = [ParseCondition()] };
    }

    private QueryCondition ParseCondition()
    {
        var target = ParseTarget();
        SkipSpace();

        if (TryWord("is"))
        {
            var negated = TryWord("not");
            if (!TryWord("null")) throw new QueryParseException("'is' has to be followed by 'null'", _at);
            return Build(target, negated ? QueryOperator.IsNotNull : QueryOperator.IsNull);
        }

        var not = TryWord("not");
        if (TryWord("between"))
        {
            var lower = ParseOperand();
            if (!TryWord("and")) throw new QueryParseException("'between' needs an 'and' after its lower bound", _at);
            var upper = ParseOperand();
            return Build(target, not ? QueryOperator.NotBetween : QueryOperator.Between) with
            {
                Value = lower,
                Value2 = upper,
            };
        }
        if (TryWord("in"))
        {
            return Build(target, not ? QueryOperator.NotIn : QueryOperator.In) with { Value = ParseSet() };
        }
        if (not) throw new QueryParseException("'not' has to be followed by 'between' or 'in'", _at);

        var word = ReadWord();
        var op = word switch
        {
            "eq" => QueryOperator.Equals,
            "ne" => QueryOperator.NotEquals,
            "gt" => QueryOperator.GreaterThan,
            "ge" => QueryOperator.GreaterThanOrEqual,
            "lt" => QueryOperator.LessThan,
            "le" => QueryOperator.LessThanOrEqual,
            "contains" => QueryOperator.Contains,
            "startswith" => QueryOperator.StartsWith,
            "endswith" => QueryOperator.EndsWith,
            _ => throw new QueryParseException($"'{word}' is not an operator", _at),
        };
        return Build(target, op) with { Value = ParseOperand() };
    }

    private static QueryCondition Build(
        (QueryFieldRef? Field, QueryFunctionCall? Call, string? Select) target, QueryOperator op)
        => new(target.Field, op) { Call = target.Call, Select = target.Select };

    private (QueryFieldRef? Field, QueryFunctionCall? Call, string? Select) ParseTarget()
    {
        SkipSpace();
        var start = _at;
        var name = ReadName();
        SkipSpace();

        if (Peek() == '(')
        {
            _at = start;
            return (null, ReadCall(), null);
        }
        if (name.IndexOf('.') >= 0) return (ReadFieldRef(name), null, null);
        // A bare name in a grouping filter is the name a selected item was given.
        return (null, null, name);
    }

    private QueryOperand ParseOperand()
    {
        SkipSpace();
        var start = _at;
        var character = Peek();

        if (character == '\'') return QueryOperand.Literal(ReadQuoted());
        if (character == '-' || character == '+') return ReadRelative();

        var name = ReadName();
        if (name.Length == 0) throw new QueryParseException("A value was expected", start);

        SkipSpace();
        if (Peek() == '(')
        {
            _at = start;
            return QueryOperand.Function(ReadCall());
        }
        if (name.IndexOf('.') >= 0) return QueryOperand.Of(ReadFieldRef(name));
        if (string.Equals(name, "null", StringComparison.OrdinalIgnoreCase)) return QueryOperand.Literal(null);
        return QueryOperand.Literal(name);
    }

    private QueryOperand ParseSet()
    {
        SkipSpace();
        if (Peek() != '(') throw new QueryParseException("A set reads as '(a,b,c)'", _at);
        _at++;

        var values = new List<string>();
        while (true)
        {
            SkipSpace();
            if (Peek() == ')') { _at++; break; }
            values.Add(Peek() == '\'' ? ReadQuoted() : ReadName());
            SkipSpace();
            if (Peek() == ',') { _at++; continue; }
            if (Peek() == ')') { _at++; break; }
            throw new QueryParseException("A set needs a comma or a closing bracket", _at);
        }
        return QueryOperand.List(values);
    }

    private QueryOperand ReadRelative()
    {
        var negative = Peek() == '-';
        _at++;
        var digits = new StringBuilder();
        while (_at < _text.Length && char.IsDigit(_text[_at])) digits.Append(_text[_at++]);
        if (digits.Length == 0) throw new QueryParseException("An offset needs an amount", _at);

        var suffix = new StringBuilder();
        while (_at < _text.Length && char.IsLetter(_text[_at])) suffix.Append(_text[_at++]);
        var unit = suffix.ToString().ToLowerInvariant() switch
        {
            "min" => QueryTimeUnit.Minute,
            "h" => QueryTimeUnit.Hour,
            "d" => QueryTimeUnit.Day,
            "w" => QueryTimeUnit.Week,
            "mon" => QueryTimeUnit.Month,
            "q" => QueryTimeUnit.Quarter,
            "y" => QueryTimeUnit.Year,
            _ => throw new QueryParseException($"'{suffix}' is not a unit of time", _at),
        };

        var amount = int.Parse(digits.ToString(), CultureInfo.InvariantCulture);
        return negative ? QueryOperand.Ago(amount, unit) : QueryOperand.FromNow(amount, unit);
    }

    private QueryFunctionCall ReadCall()
    {
        SkipSpace();
        var name = ReadName();
        SkipSpace();
        if (Peek() != '(') throw new QueryParseException($"'{name}' is not a call", _at);
        _at++;

        var arguments = new List<QueryOperand>();
        SkipSpace();
        if (Peek() == ')') { _at++; return new QueryFunctionCall(name) { Arguments = arguments }; }

        while (true)
        {
            arguments.Add(ParseOperand());
            SkipSpace();
            if (Peek() == ',') { _at++; continue; }
            if (Peek() == ')') { _at++; break; }
            throw new QueryParseException("A call needs a comma or a closing bracket", _at);
        }
        return new QueryFunctionCall(name) { Arguments = arguments };
    }

    // ---- Reading the small pieces ----------------------------------------------------------------

    private char Peek() => _at < _text.Length ? _text[_at] : '\0';

    private void SkipSpace()
    {
        while (_at < _text.Length && char.IsWhiteSpace(_text[_at])) _at++;
    }

    private string ReadName()
    {
        SkipSpace();
        var start = _at;
        while (_at < _text.Length && (char.IsLetterOrDigit(_text[_at]) || _text[_at] is '_' or '.' or '*'))
        {
            _at++;
        }
        return _text.Substring(start, _at - start);
    }

    private string ReadWord()
    {
        SkipSpace();
        var start = _at;
        while (_at < _text.Length && char.IsLetter(_text[_at])) _at++;
        if (_at == start) throw new QueryParseException("A word was expected", start);
        return _text.Substring(start, _at - start).ToLowerInvariant();
    }

    /// <summary>Consumes the word only when it is the one asked for, so a look-ahead costs nothing.</summary>
    private bool TryWord(string word)
    {
        SkipSpace();
        var mark = _at;
        var start = _at;
        while (_at < _text.Length && char.IsLetter(_text[_at])) _at++;
        if (string.Equals(_text.Substring(start, _at - start), word, StringComparison.OrdinalIgnoreCase)) return true;
        _at = mark;
        return false;
    }

    private string ReadQuoted()
    {
        _at++; // opening quote
        var value = new StringBuilder();
        while (true)
        {
            if (_at >= _text.Length) throw new QueryParseException("A quote was opened and never closed", _at);
            if (_text[_at] == '\'')
            {
                // Two quotes in a row are one quote in the value, which is how a value carries one.
                if (_at + 1 < _text.Length && _text[_at + 1] == '\'') { value.Append('\''); _at += 2; continue; }
                _at++;
                break;
            }
            value.Append(_text[_at++]);
        }
        return value.ToString();
    }

    // ---- Shared helpers --------------------------------------------------------------------------

    private static QueryFieldRef ReadFieldRef(string text)
    {
        var dot = text.IndexOf('.');
        if (dot <= 0 || dot == text.Length - 1)
        {
            throw new QueryParseException($"A field reads as 'alias.field', and '{text}' does not.");
        }
        return new QueryFieldRef(text.Substring(0, dot), text.Substring(dot + 1));
    }

    private static (QueryFieldRef? Field, QueryFunctionCall? Call) ReadValue(string text)
    {
        var body = text.Trim();
        return body.EndsWith(")", StringComparison.Ordinal)
            ? (null, new QueryHttpReader(body).ReadCall())
            : (ReadFieldRef(body), null);
    }

    private static string? LeadingCall(string text)
    {
        var open = IndexOfTop(text, '(');
        if (open <= 0) return null;
        var name = text.Substring(0, open).Trim();
        return name.All(character => char.IsLetterOrDigit(character) || character == '_') ? name : null;
    }

    private static string Inside(string call)
    {
        var open = IndexOfTop(call, '(');
        return open < 0 ? string.Empty : call.Substring(open + 1, call.Length - open - 2);
    }

    private static (string Body, string? Alias) SplitAlias(string text)
    {
        var body = text.Trim();
        var at = LastTop(body, " as ");
        return at < 0
            ? (body, null)
            : (body.Substring(0, at).Trim(), body.Substring(at + 4).Trim());
    }

    private static (string Value, QueryDateTruncation? Period) SplitPeriod(string text)
    {
        var at = IndexOfTop(text, ':');
        if (at < 0) return (text, null);
        var suffix = text.Substring(at + 1).Trim();
        return (text.Substring(0, at).Trim(), ReadEnum<QueryDateTruncation>(suffix, "period"));
    }

    private static TEnum ReadEnum<TEnum>(string text, string what) where TEnum : struct
        => Enum.TryParse<TEnum>(text, ignoreCase: true, out var value)
            ? value
            : throw new QueryParseException($"'{text}' is not a {what}.");

    /// <summary>Splits on a separator that is not inside quotes or brackets.</summary>
    private static IReadOnlyList<string> Split(string text, char separator)
    {
        var parts = new List<string>();
        var depth = 0;
        var quoted = false;
        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];
            if (character == '\'') { quoted = !quoted; continue; }
            if (quoted) continue;
            if (character == separator && depth == 0)
            {
                parts.Add(text.Substring(start, i - start));
                start = i + 1;
                continue;
            }
            if (character == '(') depth++;
            else if (character == ')') depth--;
        }
        parts.Add(text.Substring(start));
        return parts;
    }

    // The separator is looked for before the brackets are counted, so that a bracket can itself be
    // the thing being looked for.
    private static int IndexOfTop(string text, char character)
    {
        var depth = 0;
        var quoted = false;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\'') { quoted = !quoted; continue; }
            if (quoted) continue;
            if (text[i] == character && depth == 0) return i;
            if (text[i] == '(') depth++;
            else if (text[i] == ')') depth--;
        }
        return -1;
    }

    private static int LastTop(string text, string needle)
    {
        var depth = 0;
        var quoted = false;
        var found = -1;
        for (var i = 0; i + needle.Length <= text.Length; i++)
        {
            if (text[i] == '\'') { quoted = !quoted; continue; }
            if (quoted) continue;
            if (depth == 0 && string.Compare(text, i, needle, 0, needle.Length, StringComparison.OrdinalIgnoreCase) == 0)
            {
                found = i;
                continue;
            }
            if (text[i] == '(') depth++;
            else if (text[i] == ')') depth--;
        }
        return found;
    }
}

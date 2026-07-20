using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Querio.Text;

/// <summary>
/// Reads a description back into the query it describes.
/// <para>
/// Words are not a free-form language here. Everything a description can say is drawn from a closed
/// vocabulary - the labels the schema declares plus the connectives the caller supplied - so reading
/// is a matter of matching the longest thing that is known at each point rather than guessing. A
/// label containing a comma is read whole for exactly that reason.
/// </para>
/// </summary>
internal sealed class QueryTextReader
{
    private readonly string _text;
    private readonly QuerySchema _schema;
    private readonly QueryDescriptionLabels _labels;

    private readonly List<(string Alias, IReadOnlyList<QueryField> Fields)> _participants = [];
    private bool _qualify;
    private int _at;

    private QueryTextReader(string text, QuerySchema schema, QueryDescriptionLabels labels)
    {
        _text = text.Trim();
        _schema = schema;
        _labels = labels;
    }

    internal static QuerySpec Read(string description, QuerySchema schema, QueryDescriptionLabels labels)
    {
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        if (string.IsNullOrWhiteSpace(description)) throw new QueryParseException("There is nothing to read.");
        return new QueryTextReader(description, schema, labels).Run();
    }

    private QuerySpec Run()
    {
        Expect(_labels.From);
        var (root, rootCall) = ReadParticipant();
        var from = rootCall is not null
            ? QuerySource.FromFunction(rootCall, root.Alias)
            : new QuerySource(root.Entity, root.Alias);

        var joins = new List<QueryJoin>();
        while (TryClause(_labels.JoinedWith)) joins.Add(ReadJoin());

        // Everything that names a field needs to know what the query reaches, and how much of it,
        // because one table is written without qualifiers and more than one is written with them.
        _qualify = _participants.Count > 1;

        var select = TryClause(_labels.Showing) ? ReadList(ReadSelect) : [];
        var where = TryClause(_labels.Where) ? ReadFilter() : null;
        var groupBy = TryClause(_labels.GroupedBy) ? ReadList(ReadGroupBy) : [];
        var having = TryClause(_labels.Having) ? ReadFilter() : null;
        var orderBy = TryClause(_labels.OrderedBy) ? ReadList(ReadSort) : [];
        var distinct = TryClause(_labels.WithoutDuplicates);
        var offset = TryClause(_labels.Skipping) ? ReadNumber() : (int?)null;
        var limit = TryClause(_labels.First) ? ReadNumber() : (int?)null;

        SkipSpace();
        if (_at < _text.Length)
        {
            throw new QueryParseException($"'{_text.Substring(_at)}' was not expected", _at);
        }

        return new QuerySpec(from)
        {
            Joins = joins,
            Select = select,
            Where = where,
            GroupBy = groupBy,
            Having = having,
            OrderBy = orderBy,
            Distinct = distinct,
            Limit = limit,
            Offset = offset,
        };
    }

    // ---- Participants ----------------------------------------------------------------------------

    private ((string? Entity, string Alias) Source, QueryFunctionCall? Call) ReadParticipant()
    {
        var call = TryFunctionCall(QueryFunctionKind.Table);
        string? entity = null;
        IReadOnlyList<QueryField> fields;

        if (call is not null)
        {
            fields = _schema.FindFunction(call.Function)?.Columns ?? [];
        }
        else
        {
            var found = LongestLabel(_schema.Entities.Select(e => (e.Label, (object)e)))
                ?? throw new QueryParseException("An entity was expected", _at);
            var declared = (QueryEntity)found;
            entity = declared.Key;
            fields = declared.Fields;
        }

        var alias = ReadAlias();
        _participants.Add((alias, fields));
        return ((entity, alias), call);
    }

    private QueryJoin ReadJoin()
    {
        var (source, call) = ReadParticipant();
        var join = new QueryJoin(source.Entity, source.Alias) { Call = call };

        if (TryWords(_labels.Through))
        {
            var relation = LongestLabel(_schema.Relations.Select(r => (r.Label ?? r.Key, (object)r)))
                ?? throw new QueryParseException("A relation was expected", _at);
            join = join with { Relation = ((QueryRelation)relation).Key };
        }
        else if (TryWords(_labels.Matching))
        {
            // A join means the query reaches more than one source, so its matches were written with
            // qualifiers - and they are read here, before the count is otherwise known.
            _qualify = true;
            join = join with { On = ReadList(ReadJoinCondition) };
        }

        // A bracketed alias here says which side an otherwise ambiguous join hangs off.
        SkipSpace();
        if (Peek() == '(') join = join with { From = ReadAlias() };

        foreach (var pair in _labels.JoinKinds)
        {
            if (pair.Value.Length > 0 && TryWords(pair.Value)) return join with { Kind = pair.Key };
        }
        return join;
    }

    private QueryJoinCondition ReadJoinCondition()
    {
        // A join match is written with the same word as equality, since that is what it is.
        var left = ReadFieldRef();
        if (!TryWords(_labels.Operators[QueryOperator.Equals]))
        {
            throw new QueryParseException("A join match needs both sides", _at);
        }
        return new QueryJoinCondition(left, ReadFieldRef());
    }

    // ---- Selected items, grouping and ordering ---------------------------------------------------

    private QuerySelect ReadSelect()
    {
        if (TryWords(_labels.RowCount)) return Named(new QuerySelect { Aggregate = QueryAggregate.Count });

        foreach (var pair in _labels.Aggregates.OrderByDescending(p => p.Value.Length))
        {
            var item = TryAggregate(pair.Key, pair.Value);
            if (item is not null) return Named(item);
        }

        var (field, call) = ReadValue();
        return Named(new QuerySelect { Field = field, Call = call, Truncate = TryPeriod() });
    }

    private QuerySelect? TryAggregate(QueryAggregate aggregate, string pattern)
    {
        var mark = _at;
        var rank = (double?)null;

        if (aggregate == QueryAggregate.Percentile)
        {
            // "the {1} percentile of {0}" puts the rank first, so it is read before the value.
            var head = Segment(pattern, 0, "{1}");
            if (!TryWords(head)) return null;
            SkipSpace();
            var digits = ReadWhile(character => char.IsDigit(character) || character == '.');
            if (digits.Length == 0 ||
                !double.TryParse(digits, NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
            {
                _at = mark;
                return null;
            }
            rank = percent / 100d;
            if (!TryWords(Segment(pattern, pattern.IndexOf("{1}", StringComparison.Ordinal) + 3, "{0}")))
            {
                _at = mark;
                return null;
            }
        }
        else if (!TryWords(Segment(pattern, 0, "{0}")))
        {
            return null;
        }

        var distinct = TryWords(_labels.Distinct);
        try
        {
            var (field, call) = ReadValue();
            return new QuerySelect
            {
                Field = field,
                Call = call,
                Aggregate = aggregate,
                Distinct = distinct,
                Percentile = rank,
            };
        }
        catch (QueryParseException)
        {
            // The words matched but what followed was not a value, so this was not the aggregate.
            _at = mark;
            return null;
        }
    }

    private QuerySelect Named(QuerySelect item)
        => TryWords(_labels.Called) ? item with { Alias = ReadName() } : item;

    private QueryGroupBy ReadGroupBy()
    {
        var (field, call) = ReadValue();
        var group = new QueryGroupBy(field) { Call = call, Truncate = TryPeriod() };
        return TryWords(_labels.Called) ? group with { Alias = ReadName() } : group;
    }

    private QuerySort ReadSort()
    {
        var mark = _at;
        QueryFieldRef? field = null;
        QueryFunctionCall? call = null;
        string? select = null;

        try
        {
            (field, call) = ReadValue();
        }
        catch (QueryParseException)
        {
            // Nothing in the vocabulary matched, so this names something the query already showed.
            _at = mark;
            select = ReadName();
        }

        var direction = TryWords(_labels.Descending)
            ? QuerySortDirection.Descending
            : TryWords(_labels.Ascending)
                ? QuerySortDirection.Ascending
                : throw new QueryParseException("An ordering has to say which way it runs", _at);

        return new QuerySort { Field = field, Call = call, Select = select, Direction = direction };
    }

    // ---- Conditions ------------------------------------------------------------------------------

    private QueryFilterGroup ReadFilter()
    {
        SkipSpace();
        if (Peek() != '(') return new QueryFilterGroup { Conditions = [ReadCondition()] };

        _at++;
        var parts = new List<QueryFilterGroup>();
        var or = false;
        while (true)
        {
            parts.Add(ReadFilter());
            SkipSpace();
            if (Peek() == ')') { _at++; break; }
            if (TryWords(_labels.Or)) { or = true; continue; }
            if (TryWords(_labels.And)) continue;
            throw new QueryParseException("A bracketed condition was never closed", _at);
        }

        if (parts.Count == 1) return parts[0];

        var conditions = new List<QueryCondition>();
        var groups = new List<QueryFilterGroup>();
        foreach (var part in parts)
        {
            if (part.Conditions.Count == 1 && part.Groups.Count == 0) conditions.Add(part.Conditions[0]);
            else groups.Add(part);
        }
        return new QueryFilterGroup { Or = or, Conditions = conditions, Groups = groups };
    }

    private QueryCondition ReadCondition()
    {
        var mark = _at;
        QueryFieldRef? field = null;
        QueryFunctionCall? call = null;
        string? select = null;
        var type = QueryFieldType.Text;

        try
        {
            (field, call) = ReadValue();
            type = field is not null
                ? FindField(field.Alias, field.Field)?.Type ?? QueryFieldType.Text
                : _schema.FindFunction(call!.Function)?.ReturnType ?? QueryFieldType.Text;
        }
        catch (QueryParseException)
        {
            _at = mark;
            select = ReadName();
            type = QueryFieldType.Number;
        }

        // Longest first, so "is not empty" is never cut short into "is not". Matching moves the
        // cursor, so this is a plain loop rather than a query - the first hit has to be the last try.
        QueryOperator? matched = null;
        foreach (var pair in _labels.Operators.OrderByDescending(pair => pair.Value.Length))
        {
            if (!TryWords(pair.Value)) continue;
            matched = pair.Key;
            break;
        }
        if (matched is null) throw new QueryParseException("An operator was expected", _at);

        var op = matched.Value;
        var condition = new QueryCondition(field, op) { Call = call, Select = select };

        if (QueryDefaults.TakesNoValue(op)) return condition;
        if (QueryDefaults.TakesValueList(op))
        {
            var values = new List<string> { ReadQuoted() };
            while (TryWords(_labels.Or) || TryComma()) values.Add(ReadQuoted());
            return condition with { Value = QueryOperand.List(values) };
        }
        if (QueryDefaults.TakesTwoValues(op))
        {
            var lower = ReadOperand(type);
            if (!TryWords(_labels.And)) throw new QueryParseException("A range needs both bounds", _at);
            return condition with { Value = lower, Value2 = ReadOperand(type) };
        }
        return condition with { Value = ReadOperand(type) };
    }

    private QueryOperand ReadOperand(QueryFieldType type)
    {
        SkipSpace();
        if (Peek() == '"') return QueryOperand.Literal(ReadQuoted());
        if (TryWords(_labels.Nothing)) return QueryOperand.Literal(null);

        var relative = TryRelative();
        if (relative is not null) return relative;

        var call = TryFunctionCall(QueryFunctionKind.Value);
        if (call is not null) return QueryOperand.Function(call);
        return QueryOperand.Of(ReadFieldRef());
    }

    private QueryOperand? TryRelative()
    {
        foreach (var (pattern, past) in new[] { (_labels.LastWindow, true), (_labels.NextWindow, false) })
        {
            var mark = _at;
            if (!TryWords(Segment(pattern, 0, "{0}"))) continue;

            SkipSpace();
            var digits = ReadWhile(char.IsDigit);
            if (digits.Length == 0) { _at = mark; continue; }

            var between = Segment(pattern, pattern.IndexOf("{0}", StringComparison.Ordinal) + 3, "{1}");
            if (between.Length > 0 && !TryWords(between)) { _at = mark; continue; }

            QueryTimeUnit? unit = null;
            foreach (var pair in _labels.Units.OrderByDescending(pair => pair.Value.Length))
            {
                // The plural is tried first, or "day" would match the front of "days".
                if (!TryWords(pair.Value + "s") && !TryWords(pair.Value)) continue;
                unit = pair.Key;
                break;
            }
            if (unit is null) { _at = mark; continue; }

            var amount = int.Parse(digits, CultureInfo.InvariantCulture);
            return past ? QueryOperand.Ago(amount, unit.Value) : QueryOperand.FromNow(amount, unit.Value);
        }
        return null;
    }

    // ---- Values --------------------------------------------------------------------------------

    private (QueryFieldRef? Field, QueryFunctionCall? Call) ReadValue()
    {
        var call = TryFunctionCall(QueryFunctionKind.Value);
        return call is not null ? (null, call) : (ReadFieldRef(), null);
    }

    private QueryFunctionCall? TryFunctionCall(QueryFunctionKind kind)
    {
        var mark = _at;
        var found = LongestLabel(_schema.Functions
            .Where(function => function.Kind == kind)
            .Select(function => (function.Label, (object)function)));
        if (found is null) return null;

        var declared = (QueryFunction)found;
        if (declared.Parameters.Count == 0) return new QueryFunctionCall(declared.Key);
        if (!TryWords(_labels.Of)) { _at = mark; return null; }

        var arguments = new List<QueryOperand>();
        var index = 0;
        while (true)
        {
            var type = index < declared.Parameters.Count
                ? declared.Parameters[index].Type
                : QueryFieldType.Text;
            arguments.Add(ReadOperand(type));
            index++;
            if (TryWords(_labels.And) || TryComma()) continue;
            break;
        }
        return new QueryFunctionCall(declared.Key) { Arguments = arguments };
    }

    private QueryFieldRef ReadFieldRef()
    {
        // The label alone cannot choose between two participants that share it, so it is matched as
        // text first and the alias beside it decides who owns it.
        var labels = _participants
            .SelectMany(participant => participant.Fields.Select(member => (member.Label, (object)member.Label)))
            .Distinct();
        var matched = (string?)LongestLabel(labels)
            ?? throw new QueryParseException("A field was expected", _at);

        var alias = _qualify ? ReadAlias() : _participants[0].Alias;
        var owner = _participants.FirstOrDefault(participant =>
            string.Equals(participant.Alias, alias, StringComparison.OrdinalIgnoreCase));
        if (owner.Fields is null) throw new QueryParseException($"'{alias}' is not a source in this query.");

        var field = owner.Fields.FirstOrDefault(member =>
            string.Equals(member.Label, matched, StringComparison.OrdinalIgnoreCase))
            ?? throw new QueryParseException($"'{alias}' has no field labelled '{matched}'.");
        return new QueryFieldRef(alias, field.Key);
    }

    private QueryDateTruncation? TryPeriod()
    {
        var head = Segment(_labels.PerPeriod, 0, "{0}");
        var mark = _at;
        if (!TryWords(head)) return null;

        foreach (var pair in _labels.Periods.OrderByDescending(p => p.Value.Length))
        {
            if (TryWords(pair.Value)) return pair.Key;
        }
        _at = mark;
        return null;
    }

    // ---- Lists and clauses -----------------------------------------------------------------------

    private IReadOnlyList<T> ReadList<T>(Func<T> readOne)
    {
        var items = new List<T> { readOne() };
        while (true)
        {
            var mark = _at;
            if (TryWords(_labels.And)) { items.Add(readOne()); continue; }

            // The comma between two items looks exactly like the comma before the next clause, so
            // what follows it is what decides which one it was.
            if (TryComma() && !AtClause()) { items.Add(readOne()); continue; }
            _at = mark;
            break;
        }
        return items;
    }

    /// <summary>Whether a clause starts here, without consuming it.</summary>
    private bool AtClause()
    {
        var mark = _at;
        foreach (var word in new[]
        {
            _labels.From, _labels.JoinedWith, _labels.Showing, _labels.Where, _labels.GroupedBy,
            _labels.Having, _labels.OrderedBy, _labels.WithoutDuplicates, _labels.Skipping, _labels.First,
        })
        {
            if (word.Length > 0 && TryWords(word))
            {
                _at = mark;
                return true;
            }
        }
        _at = mark;
        return false;
    }

    /// <summary>Consumes the separator between clauses, then the clause's own word.</summary>
    private bool TryClause(string word)
    {
        var mark = _at;
        TryComma();
        if (TryWords(word)) return true;
        _at = mark;
        return false;
    }

    private bool TryComma()
    {
        SkipSpace();
        if (Peek() != ',') return false;
        _at++;
        return true;
    }

    private void Expect(string word)
    {
        if (!TryWords(word)) throw new QueryParseException($"'{word}' was expected", _at);
    }

    // ---- Reading the small pieces ----------------------------------------------------------------

    private char Peek() => _at < _text.Length ? _text[_at] : '\0';

    private void SkipSpace()
    {
        while (_at < _text.Length && char.IsWhiteSpace(_text[_at])) _at++;
    }

    /// <summary>Consumes the phrase only when it is there, so a look-ahead costs nothing.</summary>
    private bool TryWords(string phrase)
    {
        if (string.IsNullOrEmpty(phrase)) return true;
        SkipSpace();
        if (_at + phrase.Length > _text.Length) return false;
        if (string.Compare(_text, _at, phrase, 0, phrase.Length, StringComparison.OrdinalIgnoreCase) != 0)
        {
            return false;
        }

        // A phrase has to end where a word ends, or "is" would match the front of "is not".
        var after = _at + phrase.Length;
        if (after < _text.Length && (char.IsLetterOrDigit(_text[after]) || _text[after] == '_')
            && char.IsLetterOrDigit(phrase[phrase.Length - 1]))
        {
            return false;
        }
        _at = after;
        return true;
    }

    /// <summary>
    /// Matches the longest label that is actually there. Longest wins so that a label which begins
    /// with another one is never cut short.
    /// </summary>
    private object? LongestLabel(IEnumerable<(string Label, object Value)> candidates)
    {
        object? best = null;
        var length = 0;
        foreach (var candidate in candidates)
        {
            if (candidate.Label.Length <= length) continue;
            var mark = _at;
            if (TryWords(candidate.Label))
            {
                best = candidate.Value;
                length = candidate.Label.Length;
            }
            _at = mark;
        }
        if (best is null) return null;

        SkipSpace();
        _at += length;
        return best;
    }

    private string ReadAlias()
    {
        SkipSpace();
        if (Peek() != '(') throw new QueryParseException("An alias in brackets was expected", _at);
        _at++;
        var alias = ReadWhile(character => character != ')');
        if (Peek() != ')') throw new QueryParseException("An alias was never closed", _at);
        _at++;
        return alias.Trim();
    }

    private string ReadName()
    {
        SkipSpace();
        var name = ReadWhile(character => char.IsLetterOrDigit(character) || character == '_');
        if (name.Length == 0) throw new QueryParseException("A name was expected", _at);
        return name;
    }

    private int ReadNumber()
    {
        SkipSpace();
        var digits = ReadWhile(char.IsDigit);
        if (digits.Length == 0) throw new QueryParseException("A number was expected", _at);
        return int.Parse(digits, CultureInfo.InvariantCulture);
    }

    private string ReadQuoted()
    {
        SkipSpace();
        if (Peek() != '"') throw new QueryParseException("A quoted value was expected", _at);
        _at++;

        var value = new StringBuilder();
        while (true)
        {
            if (_at >= _text.Length) throw new QueryParseException("A quote was opened and never closed", _at);
            if (_text[_at] == '"')
            {
                // Two quotes in a row are one quote in the value, which is how a value carries one.
                if (_at + 1 < _text.Length && _text[_at + 1] == '"') { value.Append('"'); _at += 2; continue; }
                _at++;
                break;
            }
            value.Append(_text[_at++]);
        }
        return value.ToString();
    }

    private string ReadWhile(Func<char, bool> accept)
    {
        var start = _at;
        while (_at < _text.Length && accept(_text[_at])) _at++;
        return _text.Substring(start, _at - start);
    }

    private QueryField? FindField(string alias, string key)
        => _participants
            .FirstOrDefault(p => string.Equals(p.Alias, alias, StringComparison.OrdinalIgnoreCase))
            .Fields?
            .FirstOrDefault(member => string.Equals(member.Key, key, StringComparison.OrdinalIgnoreCase));

    /// <summary>The literal run of a format string between one placeholder and the next.</summary>
    private static string Segment(string pattern, int start, string until)
    {
        var end = pattern.IndexOf(until, start, StringComparison.Ordinal);
        return (end < 0 ? pattern.Substring(start) : pattern.Substring(start, end - start)).Trim();
    }
}

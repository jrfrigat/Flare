using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Querio.Language;

/// <summary>
/// Reads query text into a query.
/// <para>
/// The one thing here that is not SQL is worth the whole package: a field can be reached through a
/// foreign key, any number of hops deep. Writing <c>[r].[apiKeyId].[ownerId].[name]</c> says what a
/// person means, and each hop becomes a join the target renders however it likes - an explicit JOIN
/// in SQL, a dotted reference in 1C. The sugar disappears into the query, which is why a query
/// written back out shows the joins rather than the dots.
/// </para>
/// <para>
/// Nothing here stops at the first problem. Text being typed is broken most of the time, and an
/// editor needs every fault at once plus whatever query could still be made from the rest.
/// </para>
/// </summary>
internal sealed class QueryLanguageReader
{
    private static readonly string[] AggregateNames = ["count", "sum", "avg", "min", "max", "percentile"];

    private readonly IReadOnlyList<QueryToken> _tokens;
    private readonly QuerySchema _schema;
    private readonly List<QueryDiagnostic> _diagnostics = [];
    private readonly List<QueryJoin> _joins = [];
    private readonly Dictionary<string, string> _navigated = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(string Alias, string? Entity)> _participants = [];
    private int _at;

    private QueryLanguageReader(IReadOnlyList<QueryToken> tokens, QuerySchema schema)
    {
        _tokens = tokens;
        _schema = schema;
    }

    internal static QueryParseResult Read(string text, QuerySchema schema)
    {
        var reader = new QueryLanguageReader(QueryLexer.Split(text), schema);
        var spec = reader.Run();
        return new QueryParseResult(spec, reader._diagnostics);
    }

    private QuerySpec? Run()
    {
        // The source is read first even though it is written second: nothing selected can be
        // resolved until it is known what the query draws from.
        var selectStart = -1;
        if (TryWord("select")) selectStart = _at;
        else Error("A query starts with 'select'.", Peek());

        var distinct = false;
        if (selectStart >= 0)
        {
            distinct = TryWord("distinct");
            selectStart = _at;
            SkipToWord("from");
        }

        if (!TryWord("from"))
        {
            Error("A query has to say what it draws from ('from').", Peek());
            return null;
        }

        var root = ReadSource();
        if (root is null) return null;

        while (ReadJoin()) { }

        var where = TryWord("where") ? ReadFilter() : null;

        var groupBy = new List<QueryGroupBy>();
        if (TryWord("group"))
        {
            ExpectWord("by");
            do
            {
                var value = ReadValue();
                if (value is not null) groupBy.Add(new QueryGroupBy(value.Field) { Call = value.Call, Truncate = value.Truncate });
            }
            while (TrySymbol(","));
        }

        var having = TryWord("having") ? ReadFilter() : null;

        var orderBy = new List<QuerySort>();
        if (TryWord("order"))
        {
            ExpectWord("by");
            do
            {
                var sort = ReadSort();
                if (sort is not null) orderBy.Add(sort);
            }
            while (TrySymbol(","));
        }

        int? limit = null;
        int? offset = null;
        while (true)
        {
            if (TryWord("limit")) { limit = ReadCount("limit"); continue; }
            if (TryWord("offset")) { offset = ReadCount("offset"); continue; }
            break;
        }

        if (Peek().Kind != QueryTokenKind.End) Error("This was not expected here.", Peek());

        // Now that every participant is known, the selected items can be resolved.
        var select = new List<QuerySelect>();
        if (selectStart >= 0)
        {
            var resume = _at;
            _at = selectStart;
            if (!TrySymbol("*"))
            {
                do
                {
                    var item = ReadItem();
                    if (item is not null) select.Add(item);
                }
                while (TrySymbol(","));
            }
            if (!Peek().Is("from")) Error("This was not expected in the selected items.", Peek());
            _at = resume;
        }

        return new QuerySpec(root)
        {
            Joins = _joins,
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

    // ---- Sources and joins -----------------------------------------------------------------------

    private QuerySource? ReadSource()
    {
        var call = TryCall(QueryFunctionKind.Table);
        if (call is not null)
        {
            var functionAlias = ReadAlias(call.Function);
            _participants.Add((functionAlias, null));
            return QuerySource.FromFunction(call, functionAlias);
        }

        var start = Peek();
        var name = ReadQualifiedName();
        if (name is null) { Error("An entity was expected.", start); return null; }

        var entity = FindEntity(name);
        if (entity is null)
        {
            Error($"The schema has no entity called '{name}'.", start);
            return null;
        }

        var alias = ReadAlias(entity.Key);
        _participants.Add((alias, entity.Key));
        return new QuerySource(entity.Key, alias);
    }

    private bool ReadJoin()
    {
        var kind = QueryJoinKind.Inner;
        var mark = _at;
        if (TryWord("inner")) kind = QueryJoinKind.Inner;
        else if (TryWord("left")) kind = QueryJoinKind.Left;
        else if (TryWord("right")) kind = QueryJoinKind.Right;
        else if (TryWord("full")) kind = QueryJoinKind.Full;
        else if (TryWord("cross")) kind = QueryJoinKind.Cross;
        TryWord("outer");

        if (!TryWord("join")) { _at = mark; return false; }

        var call = TryCall(QueryFunctionKind.Table);
        QueryEntity? entity = null;
        if (call is null)
        {
            var start = Peek();
            var name = ReadQualifiedName();
            entity = name is null ? null : FindEntity(name);
            if (entity is null)
            {
                Error("An entity was expected after 'join'.", start);
                return true;
            }
        }

        var alias = ReadAlias(entity?.Key ?? call!.Function);
        _participants.Add((alias, entity?.Key));

        var join = new QueryJoin(entity?.Key, alias) { Kind = kind, Call = call };

        if (TryWord("through"))
        {
            var start = Peek();
            var relation = ReadName();
            var found = relation is null ? null : _schema.FindRelation(relation);
            if (found is null) Error($"The schema has no relation called '{relation}'.", start);
            else join = join with { Relation = found.Key };
        }
        else if (TryWord("on"))
        {
            var matches = new List<QueryJoinCondition>();
            do
            {
                var left = ReadPath();
                if (!TrySymbol("=")) Error("A join match reads as 'a.x = b.y'.", Peek());
                var right = ReadPath();
                if (left is not null && right is not null) matches.Add(new QueryJoinCondition(left, right));
            }
            while (TryWord("and"));
            join = join with { On = matches };
        }
        else if (kind != QueryJoinKind.Cross)
        {
            // Nothing said how it attaches, so the schema is asked whether exactly one relation can.
            var inferred = InferRelation(entity?.Key);
            if (inferred is null) Error("This join does not say how it attaches ('on' or 'through').", Peek());
            else join = join with { Relation = inferred };
        }

        _joins.Add(join);
        return true;
    }

    private string? InferRelation(string? entityKey)
    {
        if (entityKey is null) return null;
        var reachable = _participants
            .Where(participant => participant.Entity is not null)
            .Select(participant => participant.Entity!)
            .ToList();

        var candidates = _schema.RelationsOf(entityKey)
            .Where(relation => reachable.Any(key =>
                string.Equals(key, relation.From, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, relation.To, StringComparison.OrdinalIgnoreCase)))
            .Select(relation => relation.Key)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return candidates.Count == 1 ? candidates[0] : null;
    }

    // ---- Selected items, grouping and ordering ---------------------------------------------------

    private QuerySelect? ReadItem()
    {
        var value = ReadValue();
        if (value is null) return null;

        var alias = TryWord("as") ? ReadName() : OptionalName();
        return new QuerySelect
        {
            Field = value.Field,
            Call = value.Call,
            Aggregate = value.Aggregate,
            Distinct = value.Distinct,
            Percentile = value.Percentile,
            Truncate = value.Truncate,
            Alias = alias,
        };
    }

    private QuerySort? ReadSort()
    {
        Value? value = null;
        string? select = null;
        if (NamesAnOutput())
        {
            select = ReadName();
            if (select is null) return null;
        }
        else
        {
            value = ReadValue();
        }

        var direction = QuerySortDirection.Ascending;
        if (TryWord("desc")) direction = QuerySortDirection.Descending;
        else TryWord("asc");

        return new QuerySort
        {
            Field = value?.Field,
            Call = value?.Call,
            Select = select,
            Direction = direction,
        };
    }

    /// <summary>
    /// Whether what is here names something the query already showed rather than a value. A lone
    /// name with no dot and no bracket after it cannot be a field or a call, so it is the name of a
    /// selected item. Deciding by shape rather than by trying and backtracking matters: a failed
    /// attempt would have already reported what was wrong with it.
    /// </summary>
    private bool NamesAnOutput()
        => Peek().IsName && !At(1).IsSymbol(".") && !At(1).IsSymbol("(");

    private sealed class Value
    {
        internal QueryFieldRef? Field;
        internal QueryFunctionCall? Call;
        internal QueryAggregate? Aggregate;
        internal bool Distinct;
        internal double? Percentile;
        internal QueryDateTruncation? Truncate;
    }

    private Value? ReadValue()
    {
        var token = Peek();
        if (!token.IsName) return null;

        // A name followed by a bracket is a call: an aggregate, a period, or a declared function.
        if (token.Kind == QueryTokenKind.Word && At(1).IsSymbol("("))
        {
            var name = token.Text.ToLowerInvariant();
            if (AggregateNames.Contains(name, StringComparer.OrdinalIgnoreCase)) return ReadAggregate();
            if (string.Equals(name, "trunc", StringComparison.OrdinalIgnoreCase)) return ReadTruncation();
        }

        var call = TryCall(QueryFunctionKind.Value);
        if (call is not null) return new Value { Call = call };

        var path = ReadPath();
        return path is null ? null : new Value { Field = path };
    }

    private Value? ReadAggregate()
    {
        var name = Next().Text;
        var aggregate = (QueryAggregate)Enum.Parse(typeof(QueryAggregate), name, ignoreCase: true);
        ExpectSymbol("(");

        // Counting rows names nothing, which is what tells the two counts apart.
        if (aggregate == QueryAggregate.Count && TrySymbol("*"))
        {
            ExpectSymbol(")");
            return new Value { Aggregate = QueryAggregate.Count };
        }

        var distinct = TryWord("distinct");
        var inner = ReadValue();
        double? rank = null;

        if (aggregate == QueryAggregate.Percentile)
        {
            if (!TrySymbol(",")) Error("A percentile reads as 'percentile(value, rank)'.", Peek());
            var token = Peek();
            var text = ReadNumberText();
            if (text is null || !double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                Error("A percentile rank has to be a number between 0 and 1.", token);
            }
            else
            {
                rank = parsed;
            }
        }

        ExpectSymbol(")");
        if (inner is null) return null;
        return new Value
        {
            Field = inner.Field,
            Call = inner.Call,
            Aggregate = aggregate,
            Distinct = distinct,
            Percentile = rank,
        };
    }

    private Value? ReadTruncation()
    {
        Next();
        ExpectSymbol("(");
        var inner = ReadValue();
        if (!TrySymbol(",")) Error("A period reads as 'trunc(value, day)'.", Peek());

        var token = Peek();
        var period = ReadName();
        QueryDateTruncation? truncation = null;
        if (period is not null && Enum.TryParse<QueryDateTruncation>(period, ignoreCase: true, out var parsed))
        {
            truncation = parsed;
        }
        else
        {
            Error($"'{period}' is not a period. Use minute, hour, day, week, month, quarter or year.", token);
        }

        ExpectSymbol(")");
        if (inner is null) return null;
        return new Value { Field = inner.Field, Call = inner.Call, Truncate = truncation };
    }

    // ---- Paths, and the foreign keys they travel -------------------------------------------------

    /// <summary>
    /// Reads <c>[alias].[field]</c>, or a longer path where every name but the last is a foreign key
    /// to travel. Each hop becomes a join, and the same hop written twice reuses the one join.
    /// </summary>
    private QueryFieldRef? ReadPath()
    {
        var start = Peek();
        if (!start.IsName) { Error("A field was expected.", start); return null; }

        var parts = new List<QueryToken> { Next() };
        while (TrySymbol("."))
        {
            if (!Peek().IsName)
            {
                Error("A name was expected after the dot.", Peek());
                return null;
            }
            parts.Add(Next());
        }

        if (parts.Count < 2)
        {
            Error("A field reads as 'alias.field'.", start);
            return null;
        }

        var alias = parts[0].Text;
        var participant = _participants.FirstOrDefault(p =>
            string.Equals(p.Alias, alias, StringComparison.OrdinalIgnoreCase));
        if (participant.Alias is null)
        {
            Error($"'{alias}' is not a source in this query.", parts[0]);
            return null;
        }

        var entityKey = participant.Entity;
        for (var i = 1; i < parts.Count - 1; i++)
        {
            if (entityKey is null)
            {
                Error("A table function's columns cannot be travelled through.", parts[i]);
                return null;
            }

            var relation = FindNavigation(entityKey, parts[i].Text);
            if (relation is null)
            {
                Error($"'{parts[i].Text}' is not a foreign key on '{entityKey}'.", parts[i]);
                return null;
            }

            alias = Navigate(alias, relation);
            entityKey = relation.To;
        }

        var last = parts[parts.Count - 1];
        if (entityKey is null)
        {
            // A table function's columns are the only thing reachable through its alias.
            var function = FindFunctionColumns(participant.Alias);
            var column = function?.FirstOrDefault(member =>
                string.Equals(member.Key, last.Text, StringComparison.OrdinalIgnoreCase));
            if (column is null) { Error($"'{last.Text}' is not a column here.", last); return null; }
            return new QueryFieldRef(alias, column.Key);
        }

        var field = FindField(entityKey, last.Text);
        if (field is null)
        {
            Error($"'{entityKey}' has no field called '{last.Text}'.", last);
            return null;
        }
        return new QueryFieldRef(alias, field.Key);
    }

    /// <summary>
    /// The relation a name travels. A name matches either the relation itself or the single field
    /// holding the key, by its logical name or the physical one - somebody reading a database will
    /// write the column they can see.
    /// </summary>
    private QueryRelation? FindNavigation(string entityKey, string name)
    {
        foreach (var relation in _schema.RelationsOf(entityKey))
        {
            if (!string.Equals(relation.From, entityKey, StringComparison.OrdinalIgnoreCase)) continue;
            if (string.Equals(relation.Key, name, StringComparison.OrdinalIgnoreCase)) return relation;
            if (relation.On.Count != 1) continue;

            var field = FindField(entityKey, relation.On[0].FromField);
            if (field is null) continue;
            if (string.Equals(field.Key, name, StringComparison.OrdinalIgnoreCase)
                || string.Equals(field.PhysicalName, name, StringComparison.OrdinalIgnoreCase))
            {
                return relation;
            }
        }
        return null;
    }

    /// <summary>Adds the join a hop needs, or reuses the one an earlier hop already added.</summary>
    private string Navigate(string fromAlias, QueryRelation relation)
    {
        var key = fromAlias + "|" + relation.Key;
        if (_navigated.TryGetValue(key, out var existing)) return existing;

        var alias = FreeAlias(relation.To);
        _joins.Add(new QueryJoin(relation.To, alias)
        {
            // Travelling a key can only lose rows if it is left outer, and a report that silently
            // dropped rows because a key was empty would be answering a different question.
            Kind = QueryJoinKind.Left,
            Relation = relation.Key,
            From = fromAlias,
        });
        _participants.Add((alias, relation.To));
        _navigated[key] = alias;
        return alias;
    }

    private string FreeAlias(string entityKey)
    {
        var seed = entityKey.FirstOrDefault(char.IsLetter);
        var stem = seed == default ? "t" : char.ToLowerInvariant(seed).ToString();
        for (var suffix = 1; ; suffix++)
        {
            var candidate = stem + suffix.ToString(CultureInfo.InvariantCulture);
            if (!_participants.Any(p => string.Equals(p.Alias, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                return candidate;
            }
        }
    }

    // ---- Conditions ------------------------------------------------------------------------------

    private QueryFilterGroup? ReadFilter()
    {
        var group = ReadOr();
        return group;
    }

    private QueryFilterGroup? ReadOr()
    {
        var parts = new List<QueryFilterGroup>();
        var first = ReadAnd();
        if (first is null) return null;
        parts.Add(first);

        while (TryWord("or"))
        {
            var next = ReadAnd();
            if (next is not null) parts.Add(next);
        }
        return parts.Count == 1 ? parts[0] : Fold(parts, or: true);
    }

    private QueryFilterGroup? ReadAnd()
    {
        var parts = new List<QueryFilterGroup>();
        var first = ReadFactor();
        if (first is null) return null;
        parts.Add(first);

        while (TryWord("and"))
        {
            var next = ReadFactor();
            if (next is not null) parts.Add(next);
        }
        return parts.Count == 1 ? parts[0] : Fold(parts, or: false);
    }

    private static QueryFilterGroup Fold(IReadOnlyList<QueryFilterGroup> parts, bool or)
    {
        var conditions = new List<QueryCondition>();
        var groups = new List<QueryFilterGroup>();
        foreach (var part in parts)
        {
            if (part.Conditions.Count == 1 && part.Groups.Count == 0) conditions.Add(part.Conditions[0]);
            else groups.Add(part);
        }
        return new QueryFilterGroup { Or = or, Conditions = conditions, Groups = groups };
    }

    private QueryFilterGroup? ReadFactor()
    {
        if (TrySymbol("("))
        {
            var inner = ReadOr();
            ExpectSymbol(")");
            return inner;
        }
        var condition = ReadCondition();
        return condition is null ? null : new QueryFilterGroup { Conditions = [condition] };
    }

    private QueryCondition? ReadCondition()
    {
        Value? value = null;
        string? select = null;
        if (NamesAnOutput())
        {
            select = ReadName();
            if (select is null) return null;
        }
        else
        {
            value = ReadValue();
        }

        var type = value?.Field is not null
            ? FindFieldByRef(value.Field)?.Type ?? QueryFieldType.Text
            : value?.Call is not null
                ? _schema.FindFunction(value.Call.Function)?.ReturnType ?? QueryFieldType.Text
                : QueryFieldType.Number;

        QueryCondition Build(QueryOperator op)
            => new(value?.Field, op) { Call = value?.Call, Select = select };

        if (TryWord("is"))
        {
            var negated = TryWord("not");
            if (!TryWord("null")) Error("'is' has to be followed by 'null'.", Peek());
            return Build(negated ? QueryOperator.IsNotNull : QueryOperator.IsNull);
        }

        var not = TryWord("not");
        if (TryWord("between"))
        {
            var lower = ReadOperand(type);
            if (!TryWord("and")) Error("'between' needs an 'and' after its lower bound.", Peek());
            var upper = ReadOperand(type);
            return Build(not ? QueryOperator.NotBetween : QueryOperator.Between) with
            {
                Value = lower,
                Value2 = upper,
            };
        }
        if (TryWord("in"))
        {
            ExpectSymbol("(");
            var values = new List<string>();
            do
            {
                var token = Peek();
                var literal = ReadLiteralText();
                if (literal is null) Error("A set holds fixed values.", token);
                else values.Add(literal);
            }
            while (TrySymbol(","));
            ExpectSymbol(")");
            return Build(not ? QueryOperator.NotIn : QueryOperator.In) with { Value = QueryOperand.List(values) };
        }
        if (not) Error("'not' has to be followed by 'between' or 'in'.", Peek());

        var operatorToken = Peek();
        var comparison = ReadOperator();
        if (comparison is null)
        {
            Error("An operator was expected.", operatorToken);
            return null;
        }
        return Build(comparison.Value) with { Value = ReadOperand(type) };
    }

    private QueryOperator? ReadOperator()
    {
        var token = Peek();
        if (token.Kind == QueryTokenKind.Symbol)
        {
            var op = token.Text switch
            {
                "=" => QueryOperator.Equals,
                "<>" => QueryOperator.NotEquals,
                "!=" => QueryOperator.NotEquals,
                ">" => QueryOperator.GreaterThan,
                ">=" => QueryOperator.GreaterThanOrEqual,
                "<" => QueryOperator.LessThan,
                "<=" => QueryOperator.LessThanOrEqual,
                _ => (QueryOperator?)null,
            };
            if (op is not null) { Next(); return op; }
            return null;
        }

        if (TryWord("contains")) return QueryOperator.Contains;
        if (TryWord("startswith")) return QueryOperator.StartsWith;
        if (TryWord("endswith")) return QueryOperator.EndsWith;
        return null;
    }

    private QueryOperand? ReadOperand(QueryFieldType type)
    {
        var token = Peek();

        // "now - 30 day" keeps a window relative, so a saved query still means the last 30 days.
        if (token.Is("now"))
        {
            Next();
            var sign = TrySymbol("-") ? -1 : TrySymbol("+") ? 1 : 0;
            if (sign == 0) return QueryOperand.Literal(QueryValue.ToInvariant(DateTime.UtcNow));

            var amountToken = Peek();
            var amount = ReadNumberText();
            var unitToken = Peek();
            var unit = ReadName();
            if (amount is null || !int.TryParse(amount, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
            {
                Error("An offset from now needs an amount.", amountToken);
                return null;
            }
            var parsed = ParseUnit(unit);
            if (parsed is null)
            {
                Error($"'{unit}' is not a unit of time.", unitToken);
                return null;
            }
            return sign < 0 ? QueryOperand.Ago(count, parsed.Value) : QueryOperand.FromNow(count, parsed.Value);
        }

        var literal = ReadLiteralText();
        if (literal is not null) return QueryOperand.Literal(literal);
        if (token.Is("null")) { Next(); return QueryOperand.Literal(null); }

        var call = TryCall(QueryFunctionKind.Value);
        if (call is not null) return QueryOperand.Function(call);

        var path = ReadPath();
        return path is null ? null : QueryOperand.Of(path);
    }

    private static QueryTimeUnit? ParseUnit(string? word)
    {
        if (string.IsNullOrEmpty(word)) return null;
        var singular = word!.EndsWith("s", StringComparison.OrdinalIgnoreCase)
            ? word.Substring(0, word.Length - 1)
            : word;
        return Enum.TryParse<QueryTimeUnit>(singular, ignoreCase: true, out var unit) ? unit : null;
    }

    // ---- Names and literals ----------------------------------------------------------------------

    private QueryFunctionCall? TryCall(QueryFunctionKind kind)
    {
        var token = Peek();
        if (!token.IsName || !At(1).IsSymbol("(")) return null;

        var function = _schema.Functions.FirstOrDefault(candidate =>
            candidate.Kind == kind
            && (string.Equals(candidate.Key, token.Text, StringComparison.OrdinalIgnoreCase)
                || string.Equals(candidate.PhysicalName, token.Text, StringComparison.OrdinalIgnoreCase)));
        if (function is null) return null;

        Next();
        ExpectSymbol("(");
        var arguments = new List<QueryOperand>();
        if (!TrySymbol(")"))
        {
            var index = 0;
            do
            {
                var type = index < function.Parameters.Count
                    ? function.Parameters[index].Type
                    : QueryFieldType.Text;
                var operand = ReadOperand(type);
                if (operand is not null) arguments.Add(operand);
                index++;
            }
            while (TrySymbol(","));
            ExpectSymbol(")");
        }
        return new QueryFunctionCall(function.Key) { Arguments = arguments };
    }

    private string? ReadQualifiedName()
    {
        if (!Peek().IsName) return null;
        var parts = new List<string> { Next().Text };
        while (Peek().IsSymbol(".") && At(1).IsName && !At(2).IsSymbol("."))
        {
            // A dotted source name is a physical one such as dbo.RequestLog. A third dot would mean
            // a field path instead, so it is left alone.
            Next();
            parts.Add(Next().Text);
        }
        return string.Join(".", parts);
    }

    private string ReadAlias(string fallback)
    {
        TryWord("as");
        var token = Peek();
        if (!token.IsName || IsKeyword(token)) return FreeAlias(fallback);
        return Next().Text;
    }

    private string? OptionalName()
    {
        var token = Peek();
        return token.IsName && !IsKeyword(token) ? Next().Text : null;
    }

    private static bool IsKeyword(QueryToken token)
        => token.Kind == QueryTokenKind.Word
            && (token.Is("from") || token.Is("where") || token.Is("group") || token.Is("having")
                || token.Is("order") || token.Is("limit") || token.Is("offset") || token.Is("join")
                || token.Is("inner") || token.Is("left") || token.Is("right") || token.Is("full")
                || token.Is("cross") || token.Is("on") || token.Is("through") || token.Is("and")
                || token.Is("or") || token.Is("asc") || token.Is("desc") || token.Is("as"));

    private string? ReadName()
    {
        var token = Peek();
        if (!token.IsName) return null;
        Next();
        return token.Text;
    }

    private string? ReadNumberText()
    {
        var token = Peek();
        if (token.Kind != QueryTokenKind.Number) return null;
        Next();
        return token.Text;
    }

    private string? ReadLiteralText()
    {
        var token = Peek();
        switch (token.Kind)
        {
            case QueryTokenKind.Text:
            case QueryTokenKind.Number:
                Next();
                return token.Text;
            default:
                if (!token.Is("true") && !token.Is("false")) return null;
                Next();
                return token.Text.ToLowerInvariant();
        }
    }

    private int? ReadCount(string what)
    {
        var token = Peek();
        var text = ReadNumberText();
        if (text is null || !int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            Error($"'{what}' needs a whole number.", token);
            return null;
        }
        return value;
    }

    // ---- Schema lookups --------------------------------------------------------------------------

    /// <summary>
    /// Finds an entity by the logical key or by the physical name. Both are accepted on purpose: a
    /// person reading a database writes what they can see there, and the query stores the logical
    /// name either way, so the text stays portable.
    /// </summary>
    private QueryEntity? FindEntity(string name)
        => _schema.FindEntity(name)
            ?? _schema.Entities.FirstOrDefault(entity =>
                string.Equals(entity.PhysicalName, name, StringComparison.OrdinalIgnoreCase))
            ?? _schema.Entities.FirstOrDefault(entity =>
                string.Equals(Tail(entity.PhysicalName), Tail(name), StringComparison.OrdinalIgnoreCase));

    private static string Tail(string name)
    {
        var dot = name.LastIndexOf('.');
        return dot < 0 ? name : name.Substring(dot + 1);
    }

    private QueryField? FindField(string entityKey, string name)
    {
        var entity = _schema.FindEntity(entityKey);
        if (entity is null) return null;
        return entity.FindField(name)
            ?? entity.Fields.FirstOrDefault(field =>
                string.Equals(field.PhysicalName, name, StringComparison.OrdinalIgnoreCase));
    }

    private QueryField? FindFieldByRef(QueryFieldRef reference)
    {
        var participant = _participants.FirstOrDefault(p =>
            string.Equals(p.Alias, reference.Alias, StringComparison.OrdinalIgnoreCase));
        return participant.Entity is null ? null : FindField(participant.Entity, reference.Field);
    }

    private IReadOnlyList<QueryField>? FindFunctionColumns(string alias)
    {
        var join = _joins.FirstOrDefault(candidate =>
            string.Equals(candidate.Alias, alias, StringComparison.OrdinalIgnoreCase));
        var call = join?.Call;
        return call is null ? null : _schema.FindFunction(call.Function)?.Columns;
    }

    // ---- Moving through the tokens ---------------------------------------------------------------

    private QueryToken Peek() => At(0);

    private QueryToken At(int ahead)
        => _at + ahead < _tokens.Count ? _tokens[_at + ahead] : _tokens[_tokens.Count - 1];

    private QueryToken Next() => _at < _tokens.Count - 1 ? _tokens[_at++] : _tokens[_tokens.Count - 1];

    private bool TryWord(string word)
    {
        if (!Peek().Is(word)) return false;
        Next();
        return true;
    }

    private void ExpectWord(string word)
    {
        if (!TryWord(word)) Error($"'{word}' was expected.", Peek());
    }

    private bool TrySymbol(string symbol)
    {
        if (!Peek().IsSymbol(symbol)) return false;
        Next();
        return true;
    }

    private void ExpectSymbol(string symbol)
    {
        if (!TrySymbol(symbol)) Error($"'{symbol}' was expected.", Peek());
    }

    private void SkipToWord(string word)
    {
        while (Peek().Kind != QueryTokenKind.End && !Peek().Is(word)) Next();
    }

    private void Error(string message, QueryToken token)
        => _diagnostics.Add(QueryDiagnostic.Error(message, token));
}

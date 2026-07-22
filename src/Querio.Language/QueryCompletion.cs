using System.Collections.Generic;
using System.Linq;

namespace Querio.Language;

/// <summary>What a suggestion stands for, so an editor can show a fitting icon.</summary>
public enum QueryCompletionKind
{
    /// <summary>A word the language itself uses.</summary>
    Keyword,

    /// <summary>An entity the query can draw from.</summary>
    Entity,

    /// <summary>A field of something already in the query.</summary>
    Field,

    /// <summary>A foreign key that can be travelled to reach another entity's fields.</summary>
    Navigation,

    /// <summary>An alias already in the query.</summary>
    Alias,

    /// <summary>A declared relation a join can travel.</summary>
    Relation,

    /// <summary>A declared function.</summary>
    Function,

    /// <summary>An aggregate.</summary>
    Aggregate,

    /// <summary>The name something selected was given.</summary>
    Output,

    /// <summary>A member of an enumerated field, or another fixed value worth offering.</summary>
    Value,
}

/// <summary>
/// One thing that could be written where the caret is.
/// </summary>
/// <param name="Text">What to insert.</param>
/// <param name="Label">What to show. Usually the same as <paramref name="Text"/>, unbracketed.</param>
/// <param name="Kind">What the suggestion stands for.</param>
public sealed record QueryCompletion(string Text, string Label, QueryCompletionKind Kind)
{
    /// <summary>A fuller caption: the entity a field belongs to, what a relation reaches.</summary>
    public string? Detail { get; init; }

    /// <summary>
    /// Where the text being completed starts, so a caller replaces the partial word rather than
    /// appending to it.
    /// </summary>
    public int ReplaceStart { get; init; }

    /// <summary>How many characters the insertion replaces.</summary>
    public int ReplaceLength { get; init; }
}

/// <summary>
/// Answers what could be written at a position in query text.
/// <para>
/// This lives in the core rather than in an editor because it is the same question a designer asks
/// through <c>QueryChoices</c>, only asked at a caret instead of at a dropdown. An editor that
/// worked it out for itself would eventually disagree with the rest of the product about what is
/// allowed.
/// </para>
/// <para>
/// It reads text that does not parse, which is the whole point: a caret sits in half-typed text
/// nearly all the time. What is known comes from the partial query the reader still returns.
/// </para>
/// </summary>
public static class QueryCompletionEngine
{
    private static readonly string[] AggregateWords = ["count", "sum", "avg", "min", "max", "percentile"];

    private static readonly string[] AfterQuery =
        ["where", "group by", "having", "order by", "limit", "offset", "join", "left join"];

    /// <summary>Suggests what could be written at a caret.</summary>
    /// <param name="text">The query text, complete or not.</param>
    /// <param name="caret">Zero-based caret offset into the text.</param>
    /// <param name="schema">The schema the query is written against.</param>
    public static IReadOnlyList<QueryCompletion> Suggest(string text, int caret, QuerySchema schema)
    {
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        text ??= string.Empty;
        caret = Math.Max(0, Math.Min(caret, text.Length));

        var tokens = QueryLexer.Split(text);
        var spec = QueryLanguage.Read(text, schema).Spec;
        var context = new Context(text, caret, tokens, schema, spec);

        // A path is decided first: after a dot, nothing else can be meant, and the answer depends on
        // what the names before the dot resolved to rather than on which clause this is.
        var path = context.PathBeforeCaret();
        if (path is not null) return context.Rank(Path(context, path));

        var clause = context.Clause();

        // A source clause stops offering entities once it has one: what follows a named source is
        // the next clause, and going on suggesting tables there would be noise.
        if (clause is "from" or "join" && context.SourceIsNamed()) clause = "end";

        return context.Rank(clause switch
        {
            "select" or "group" or "order" or "having" => Values(context, clause),
            "from" or "join" => Sources(context, clause),
            "where" => Values(context, clause),
            _ => Start(context),
        });
    }

    private static IEnumerable<QueryCompletion> Start(Context context)
    {
        if (context.Spec is null) yield return new QueryCompletion("select", "select", QueryCompletionKind.Keyword);
        foreach (var word in AfterQuery) yield return new QueryCompletion(word, word, QueryCompletionKind.Keyword);
    }

    private static IEnumerable<QueryCompletion> Sources(Context context, string clause)
    {
        // Right after 'join', a relation is worth offering as much as an entity is.
        foreach (var entity in context.Schema.Entities)
        {
            yield return new QueryCompletion(Bracket(entity.Key), entity.Key, QueryCompletionKind.Entity)
            {
                Detail = entity.Label,
            };
        }
        foreach (var function in context.Schema.Functions.Where(f => f.Kind == QueryFunctionKind.Table))
        {
            yield return new QueryCompletion(function.Key + "(", function.Key, QueryCompletionKind.Function)
            {
                Detail = function.Label,
            };
        }
        if (clause != "join") yield break;

        foreach (var relation in context.Schema.Relations)
        {
            yield return new QueryCompletion(Bracket(relation.Key), relation.Key, QueryCompletionKind.Relation)
            {
                Detail = $"{relation.From} -> {relation.To}",
            };
        }
    }

    private static IEnumerable<QueryCompletion> Values(Context context, string clause)
    {
        foreach (var participant in context.Participants)
        {
            yield return new QueryCompletion(Bracket(participant.Alias), participant.Alias, QueryCompletionKind.Alias)
            {
                Detail = participant.Entity ?? participant.Function,
            };
        }

        // A field is reachable unqualified only when one source could have meant it; otherwise the
        // alias has to be written first, and offering the bare name would be a trap.
        if (context.Participants.Count == 1)
        {
            foreach (var field in context.FieldsOf(context.Participants[0]))
            {
                yield return Field(field, context.Participants[0].Alias);
            }
        }

        foreach (var word in AggregateWords)
        {
            yield return new QueryCompletion(word + "(", word, QueryCompletionKind.Aggregate);
        }
        yield return new QueryCompletion("trunc(", "trunc", QueryCompletionKind.Aggregate)
        {
            Detail = "collapse a moment to a period",
        };

        foreach (var function in context.Schema.Functions.Where(f => f.Kind == QueryFunctionKind.Value))
        {
            yield return new QueryCompletion(function.Key + "(", function.Key, QueryCompletionKind.Function)
            {
                Detail = function.Label,
            };
        }

        if (clause is "having" or "order")
        {
            foreach (var output in context.Outputs)
            {
                yield return new QueryCompletion(Bracket(output), output, QueryCompletionKind.Output);
            }
        }
    }

    /// <summary>
    /// What can follow a dot. Either the fields of whatever the path has reached so far, or another
    /// key to travel - which is what makes a chain of any length possible to type.
    /// </summary>
    private static IEnumerable<QueryCompletion> Path(Context context, PathTarget target)
    {
        if (target.EntityKey is null)
        {
            foreach (var column in target.Columns ?? []) yield return Field(column, target.Alias);
            yield break;
        }

        var entity = context.Schema.FindEntity(target.EntityKey);
        if (entity is null) yield break;

        foreach (var field in entity.Fields) yield return Field(field, target.Alias);

        foreach (var relation in context.Schema.RelationsOf(entity.Key))
        {
            if (!string.Equals(relation.From, entity.Key, StringComparison.OrdinalIgnoreCase)) continue;
            var reached = context.Schema.FindEntity(relation.To);
            if (reached is null) continue;

            // A single-column key can be travelled by naming the field, which reads better; a
            // composite one has no single field, so the relation is the only way through.
            var through = relation.On.Count == 1
                ? entity.FindField(relation.On[0].FromField)?.Key ?? relation.Key
                : relation.Key;

            yield return new QueryCompletion(Bracket(through), through, QueryCompletionKind.Navigation)
            {
                Detail = $"-> {reached.Label}",
            };
        }
    }

    private static QueryCompletion Field(QueryField field, string alias)
        => new(Bracket(field.Key), field.Key, QueryCompletionKind.Field)
        {
            Detail = field.Label,
        };

    private static string Bracket(string name) => "[" + name.Replace("]", "]]") + "]";

    /// <summary>Where a path has reached: an entity, or a table function's columns.</summary>
    private sealed record PathTarget(string Alias, string? EntityKey, IReadOnlyList<QueryField>? Columns);

    private sealed class Context
    {
        private readonly string _text;
        private readonly int _caret;
        private readonly IReadOnlyList<QueryToken> _tokens;

        internal Context(
            string text, int caret, IReadOnlyList<QueryToken> tokens, QuerySchema schema, QuerySpec? spec)
        {
            _text = text;
            _caret = caret;
            _tokens = tokens;
            Schema = schema;
            Spec = spec;

            Participants = [];
            if (spec is null) return;

            Participants.Add(new Participant(spec.From.Alias, spec.From.Entity, spec.From.Call?.Function));
            foreach (var join in spec.Joins)
            {
                Participants.Add(new Participant(join.Alias, join.Entity, join.Call?.Function));
            }
        }

        internal QuerySchema Schema { get; }

        internal QuerySpec? Spec { get; }

        internal List<Participant> Participants { get; }

        internal IReadOnlyList<string> Outputs
            => Spec?.Select.Where(item => !string.IsNullOrEmpty(item.Alias)).Select(item => item.Alias!).ToList() ?? [];

        internal sealed record Participant(string Alias, string? Entity, string? Function);

        internal IReadOnlyList<QueryField> FieldsOf(Participant participant)
        {
            if (participant.Entity is not null) return Schema.FindEntity(participant.Entity)?.Fields ?? [];
            if (participant.Function is not null) return Schema.FindFunction(participant.Function)?.Columns ?? [];
            return [];
        }

        /// <summary>The token the caret is sitting in or immediately after, when it is a name.</summary>
        private int PartialIndex()
        {
            for (var i = _tokens.Count - 1; i >= 0; i--)
            {
                var token = _tokens[i];
                if (token.Kind == QueryTokenKind.End) continue;
                if (token.IsName && token.Touches(_caret) && _caret > token.Start) return i;
                if (token.End <= _caret) return -1;
            }
            return -1;
        }

        /// <summary>
        /// The names written before the caret, resolved to whatever they reached. Null when the
        /// caret is not in a path at all.
        /// </summary>
        internal PathTarget? PathBeforeCaret()
        {
            var index = PartialIndex();
            var at = index >= 0 ? index : FirstTokenAfterCaret() - 1;
            if (index >= 0) at = index - 1;

            // A path only exists when a dot sits immediately before what is being typed.
            if (at < 0 || at >= _tokens.Count || !_tokens[at].IsSymbol(".")) return null;

            var names = new List<QueryToken>();
            var walk = at;
            while (walk >= 0)
            {
                if (_tokens[walk].IsSymbol(".")) { walk--; continue; }
                if (!_tokens[walk].IsName) break;
                names.Insert(0, _tokens[walk]);
                walk--;
                if (walk < 0 || !_tokens[walk].IsSymbol(".")) break;
            }
            if (names.Count == 0) return null;

            var participant = Participants.FirstOrDefault(candidate =>
                string.Equals(candidate.Alias, names[0].Text, StringComparison.OrdinalIgnoreCase));
            if (participant is null) return null;

            if (participant.Entity is null)
            {
                // A table function's columns are the end of the road; nothing can be travelled.
                return names.Count == 1
                    ? new PathTarget(participant.Alias, null, FieldsOf(participant))
                    : null;
            }

            var entityKey = participant.Entity;
            for (var i = 1; i < names.Count; i++)
            {
                var relation = Travel(entityKey, names[i].Text);
                if (relation is null) return null;
                entityKey = relation.To;
            }
            return new PathTarget(participant.Alias, entityKey, null);
        }

        private QueryRelation? Travel(string entityKey, string name)
        {
            var entity = Schema.FindEntity(entityKey);
            if (entity is null) return null;

            foreach (var relation in Schema.RelationsOf(entityKey))
            {
                if (!string.Equals(relation.From, entityKey, StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(relation.Key, name, StringComparison.OrdinalIgnoreCase)) return relation;
                if (relation.On.Count != 1) continue;

                var field = entity.FindField(relation.On[0].FromField);
                if (field is null) continue;
                if (string.Equals(field.Key, name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(field.PhysicalName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return relation;
                }
            }
            return null;
        }

        private int FirstTokenAfterCaret()
        {
            for (var i = 0; i < _tokens.Count; i++)
            {
                if (_tokens[i].Start >= _caret) return i;
            }
            return _tokens.Count - 1;
        }

        /// <summary>
        /// Whether the source clause the caret is in already names something. Decided by counting
        /// the names written since the clause word, since two of them - the source and its alias -
        /// mean there is nothing left to name.
        /// </summary>
        internal bool SourceIsNamed()
        {
            var names = 0;
            var counting = false;
            foreach (var token in _tokens)
            {
                if (token.Start >= _caret) break;
                if (token.Kind == QueryTokenKind.Word
                    && token.Text.ToLowerInvariant() is "from" or "join")
                {
                    counting = true;
                    names = 0;
                    continue;
                }
                if (!counting) continue;
                if (token.IsName && !token.Is("as") && !token.Is("through")) names++;
            }
            return names >= 2;
        }

        /// <summary>The clause the caret is in, decided by the last clause word before it.</summary>
        internal string Clause()
        {
            var clause = string.Empty;
            foreach (var token in _tokens)
            {
                if (token.Start >= _caret) break;
                if (token.Kind != QueryTokenKind.Word) continue;

                var word = token.Text.ToLowerInvariant();
                if (word is "select" or "from" or "where" or "group" or "having" or "order" or "join")
                {
                    clause = word;
                }
                else if (word is "limit" or "offset")
                {
                    clause = "end";
                }
            }
            return clause;
        }

        /// <summary>
        /// Keeps what matches the partial word, and says which characters to replace. Matching is
        /// on what is written rather than fuzzy, since a wrong guess in a query costs more than a
        /// missing suggestion.
        /// </summary>
        internal IReadOnlyList<QueryCompletion> Rank(IEnumerable<QueryCompletion> candidates)
        {
            var index = PartialIndex();
            var partial = index >= 0 ? _tokens[index] : default;
            var written = index >= 0 ? partial.Text : string.Empty;
            var start = index >= 0 ? partial.Start : _caret;
            var length = index >= 0 ? partial.Length : 0;

            var kept = new List<QueryCompletion>();
            foreach (var candidate in candidates)
            {
                if (written.Length > 0
                    && candidate.Label.IndexOf(written, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }
                kept.Add(candidate with { ReplaceStart = start, ReplaceLength = length });
            }

            // What the word starts with beats what merely contains it, then shortest, then by name -
            // a stable order, because a list that reshuffles under the caret is unusable.
            return kept
                .OrderByDescending(candidate =>
                    written.Length > 0
                    && candidate.Label.StartsWith(written, StringComparison.OrdinalIgnoreCase))
                .ThenBy(candidate => candidate.Label.Length)
                .ThenBy(candidate => candidate.Label, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}

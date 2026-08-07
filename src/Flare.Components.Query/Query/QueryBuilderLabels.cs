namespace Flare.Components;

/// <summary>
/// Every caption the query designer shows, in one place. Gathering them here keeps the component's
/// parameter list about behaviour rather than wording, and lets a host swap the whole set for a
/// translated one in a single assignment.
/// </summary>
public sealed record QueryBuilderLabels
{
    /// <summary>The set used when a host supplies none.</summary>
    public static QueryBuilderLabels Default { get; } = new();

    /// <summary>Heading of the section choosing what the query draws from.</summary>
    public string Source { get; init; } = "Source";

    /// <summary>Heading of the column grid.</summary>
    public string Columns { get; init; } = "Columns";

    /// <summary>Heading of the row-condition section.</summary>
    public string Filter { get; init; } = "Filter";

    /// <summary>Heading of the group-condition section.</summary>
    public string GroupFilter { get; init; } = "Filter groups";

    /// <summary>Heading of the section holding distinct, limit and offset.</summary>
    public string Options { get; init; } = "Options";

    /// <summary>Heading of the list of problems found in the query.</summary>
    public string Problems { get; init; } = "Problems";

    /// <summary>Caption of the button adding a joined entity.</summary>
    public string AddJoin { get; init; } = "Add join";

    /// <summary>Caption of the button adding a column.</summary>
    public string AddColumn { get; init; } = "Add column";

    /// <summary>Caption of the button that submits the finished query.</summary>
    public string Submit { get; init; } = "Run";

    /// <summary>Accessible label for the buttons that remove a row.</summary>
    public string Remove { get; init; } = "Remove";

    /// <summary>Column heading for what a row returns.</summary>
    public string Value { get; init; } = "Value";

    /// <summary>Column heading for the aggregate applied to a row.</summary>
    public string Aggregate { get; init; } = "Aggregate";

    /// <summary>Column heading for the period a timestamp is collapsed into.</summary>
    public string Period { get; init; } = "Period";

    /// <summary>Column heading for the output name of a row.</summary>
    public string OutputName { get; init; } = "Name";

    /// <summary>Column heading for whether a row takes part in the grouping.</summary>
    public string Group { get; init; } = "Group";

    /// <summary>Column heading for the ordering applied to a row.</summary>
    public string Sort { get; init; } = "Sort";

    /// <summary>Caption for the alias of a participant.</summary>
    public string Alias { get; init; } = "Alias";

    /// <summary>Caption for the relation a join traverses.</summary>
    public string Relation { get; init; } = "Relation";

    /// <summary>Caption for how unmatched rows are treated.</summary>
    public string JoinKind { get; init; } = "Join";

    /// <summary>Caption for the toggle dropping duplicate rows.</summary>
    public string Distinct { get; init; } = "Distinct";

    /// <summary>Caption for the maximum number of rows.</summary>
    public string Limit { get; init; } = "Limit";

    /// <summary>Caption for the number of rows skipped.</summary>
    public string Offset { get; init; } = "Offset";

    /// <summary>Text shown for the option meaning no aggregate, period or ordering.</summary>
    public string None { get; init; } = "None";

    /// <summary>Text shown when the query has no problems.</summary>
    public string NoProblems { get; init; } = "The query is valid.";

    /// <summary>Caption for the AND connector in the condition tree.</summary>
    public string And { get; init; } = "AND";

    /// <summary>Caption for the OR connector in the condition tree.</summary>
    public string Or { get; init; } = "OR";

    /// <summary>Caption of the button adding a condition.</summary>
    public string AddCondition { get; init; } = "Condition";

    /// <summary>Caption of the button adding a nested condition node.</summary>
    public string AddGroup { get; init; } = "Group";

    /// <summary>Placeholder for a single condition value.</summary>
    public string ValueHint { get; init; } = "Value";

    /// <summary>Placeholder for a comma-separated set of condition values.</summary>
    public string ValuesHint { get; init; } = "a, b, c";

    /// <summary>Caption for the toggle making a timestamp comparison relative to now.</summary>
    public string Relative { get; init; } = "Relative";

    /// <summary>Ascending ordering.</summary>
    public string Ascending { get; init; } = "Ascending";

    /// <summary>Descending ordering.</summary>
    public string Descending { get; init; } = "Descending";

    /// <summary>Shown in place of the designer when no schema was supplied.</summary>
    public string SchemaMissing { get; init; } = "A schema is required.";
}

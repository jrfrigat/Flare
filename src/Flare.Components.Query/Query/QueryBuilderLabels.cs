using System.Globalization;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.Localization;

namespace Flare.Components;

/// <summary>
/// Every caption the query designer shows, in one place. Gathering them here keeps the component's
/// parameter list about behaviour rather than wording, and lets a host swap the whole set for a
/// translated one in a single assignment.
/// </summary>
public sealed record QueryBuilderLabels
{
    // The connectors belong to the query language, not to the interface: a translated designer still
    // writes the same AND/OR tokens into the spec, so no resource holds them.
    private const string AndToken = "AND";
    private const string OrToken = "OR";

    /// <summary>The set used when a host supplies none, worded in the current UI culture.</summary>
    public static QueryBuilderLabels Default => For(CultureInfo.CurrentUICulture);

    /// <summary>
    /// Every caption from the package's resources in <paramref name="culture"/>. The AND and OR
    /// connectors are syntax tokens and stay the same in every culture.
    /// </summary>
    public static QueryBuilderLabels For(CultureInfo culture)
    {
        var context = new FlareLocalization();
        context.SetCulture(culture);
        return From(context.GetLocalizer<QueryStrings>());
    }

    /// <summary>Builds a caption snapshot from the application's standard localizer and resource fallback.</summary>
    /// <param name="text">Live query-resource localizer; create a new snapshot when its culture changes.</param>
    public static QueryBuilderLabels From(IStringLocalizer<QueryStrings> text) => new()
    {
        Source = text["Query_Source"].Value,
        Columns = text["Query_Columns"].Value,
        Filter = text["Query_Filter"].Value,
        GroupFilter = text["Query_GroupFilter"].Value,
        Options = text["Query_Options"].Value,
        Problems = text["Query_Problems"].Value,
        AddJoin = text["Query_AddJoin"].Value,
        AddColumn = text["Query_AddColumn"].Value,
        Submit = text["Query_Submit"].Value,
        Remove = text["Query_Remove"].Value,
        Value = text["Query_Value"].Value,
        Aggregate = text["Query_Aggregate"].Value,
        Period = text["Query_Period"].Value,
        OutputName = text["Query_OutputName"].Value,
        Group = text["Query_Group"].Value,
        Sort = text["Query_Sort"].Value,
        Alias = text["Query_Alias"].Value,
        Relation = text["Query_Relation"].Value,
        JoinKind = text["Query_JoinKind"].Value,
        Distinct = text["Query_Distinct"].Value,
        Limit = text["Query_Limit"].Value,
        Offset = text["Query_Offset"].Value,
        None = text["Query_None"].Value,
        NoProblems = text["Query_NoProblems"].Value,
        And = AndToken,
        Or = OrToken,
        AddCondition = text["Query_AddCondition"].Value,
        AddGroup = text["Query_AddGroup"].Value,
        ValueHint = text["Query_ValueHint"].Value,
        ValuesHint = text["Query_ValuesHint"].Value,
        Relative = text["Query_Relative"].Value,
        Ascending = text["Query_Ascending"].Value,
        Descending = text["Query_Descending"].Value,
        SchemaMissing = text["Query_SchemaMissing"].Value,
        OperatorContains = text["Query_OperatorContains"].Value,
        OperatorStartsWith = text["Query_OperatorStartsWith"].Value,
        OperatorEndsWith = text["Query_OperatorEndsWith"].Value,
        OperatorBetween = text["Query_OperatorBetween"].Value,
        OperatorNotBetween = text["Query_OperatorNotBetween"].Value,
        OperatorIn = text["Query_OperatorIn"].Value,
        OperatorNotIn = text["Query_OperatorNotIn"].Value,
        OperatorIsNull = text["Query_OperatorIsNull"].Value,
        OperatorIsNotNull = text["Query_OperatorIsNotNull"].Value,
    };

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

    /// <summary>The AND connector in the condition tree. A query-language token, not a translated caption.</summary>
    public string And { get; init; } = AndToken;

    /// <summary>The OR connector in the condition tree. A query-language token, not a translated caption.</summary>
    public string Or { get; init; } = OrToken;

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

    /// <summary>Caption for the contains condition operator.</summary>
    public string OperatorContains { get; init; } = "contains";

    /// <summary>Caption for the starts with condition operator.</summary>
    public string OperatorStartsWith { get; init; } = "starts with";

    /// <summary>Caption for the ends with condition operator.</summary>
    public string OperatorEndsWith { get; init; } = "ends with";

    /// <summary>Caption for the between condition operator.</summary>
    public string OperatorBetween { get; init; } = "between";

    /// <summary>Caption for the not between condition operator.</summary>
    public string OperatorNotBetween { get; init; } = "not between";

    /// <summary>Caption for the in condition operator.</summary>
    public string OperatorIn { get; init; } = "in";

    /// <summary>Caption for the not in condition operator.</summary>
    public string OperatorNotIn { get; init; } = "not in";

    /// <summary>Caption for the is empty condition operator.</summary>
    public string OperatorIsNull { get; init; } = "is empty";

    /// <summary>Caption for the is not empty condition operator.</summary>
    public string OperatorIsNotNull { get; init; } = "is not empty";
}

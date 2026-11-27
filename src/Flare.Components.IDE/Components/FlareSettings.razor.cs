using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Flare.Components.IDE;

/// <summary>Displays an IDE settings document with searchable navigation and a page supplied by the host.</summary>
public partial class FlareSettings
{
    private readonly HashSet<string> _collapsed = new(StringComparer.Ordinal);
    private IReadOnlyList<FlareSettingsCategory> _visible = [];
    private FlareSettingsCategory? _selected;
    private string _query = string.Empty;
    private bool _filtering;

    /// <summary>The settings categories. IDs must be unique throughout the tree and stable across updates.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<FlareSettingsCategory> Categories { get; set; } = [];

    /// <summary>The ID of the selected category. A missing or invalid ID displays the first selectable category.</summary>
    [Parameter] public string? SelectedId { get; set; }

    /// <summary>Requests selection of a category. The host accepts it by updating <see cref="SelectedId"/>.</summary>
    [Parameter] public EventCallback<string> SelectedIdChanged { get; set; }

    /// <summary>Content of the selected category. Compose property editors with <see cref="FlarePropertyGrid"/> or supply a page.</summary>
    [Parameter, EditorRequired] public RenderFragment<FlareSettingsCategory>? PageContent { get; set; }

    /// <summary>Content shown when there are no selectable categories.</summary>
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    /// <summary>Accessible name of the category tree. When omitted it uses the localized settings label.</summary>
    [Parameter] public string? NavigationLabel { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Ide.Settings.Root;

    private IStringLocalizer<IdeStrings> IdeText => Localizer<IdeStrings>();

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var pages = SelectablePages(Categories);
        _selected = pages.FirstOrDefault(category => category.Id == SelectedId) ??
            SelectablePages(Categories).FirstOrDefault();
        RefreshSearch();
    }

    private void SetQuery(string? query)
    {
        _query = query ?? string.Empty;
        RefreshSearch();
    }

    private void RefreshSearch()
    {
        var term = _query.Trim();
        _filtering = term.Length > 0;
        _visible = _filtering ? Filter(Categories, term) : Categories;
    }

    private List<FlareSettingsCategory> Filter(IReadOnlyList<FlareSettingsCategory> categories, string term)
    {
        var matches = new List<FlareSettingsCategory>(categories.Count);
        foreach (var category in categories)
        {
            if (Matches(category.Label, term) || Matches(category.Description, term) ||
                category.SearchTerms.Any(text => Matches(text, term)))
            {
                matches.Add(category);
                continue;
            }

            var children = Filter(category.Children, term);
            if (children.Count > 0)
            {
                matches.Add(category with { Children = children });
            }
        }
        return matches;
    }

    private bool Matches(string? text, string term) =>
        text is not null && InterfaceCulture.CompareInfo.IndexOf(text, term,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

    private static IEnumerable<FlareSettingsCategory> SelectablePages(IReadOnlyList<FlareSettingsCategory> categories)
    {
        foreach (var category in categories)
        {
            if (category.Selectable)
            {
                yield return category;
            }
            foreach (var child in SelectablePages(category.Children))
            {
                yield return child;
            }
        }
    }

    private bool IsExpanded(FlareSettingsCategory category) => _filtering || !_collapsed.Contains(category.Id);

    private void SetExpanded(FlareSettingsCategory category, bool expanded)
    {
        if (_filtering)
        {
            return;
        }
        if (expanded)
        {
            _collapsed.Remove(category.Id);
        }
        else
        {
            _collapsed.Add(category.Id);
        }
    }

    private Task SelectAsync(FlareSettingsCategory category, bool selected) =>
        selected && category.Selectable && _selected?.Id != category.Id
            ? SelectedIdChanged.InvokeAsync(category.Id)
            : Task.CompletedTask;
}

using System.Globalization;
using Flare.Components.Resources;
using Microsoft.Extensions.Localization;

namespace Flare.Components;

internal sealed class QueryLabelsCache
{
    private QueryBuilderLabels? _defaults;
    private FlareLocalizationState? _state;
    private CultureInfo? _culture;

    public QueryBuilderLabels Resolve(QueryBuilderLabels? explicitLabels, FlareLocalizationState? state,
        CultureInfo culture, IStringLocalizer<QueryStrings> text)
    {
        if (explicitLabels is not null) return explicitLabels;
        if (_defaults is null || !ReferenceEquals(_state, state) || !ReferenceEquals(_culture, culture))
        {
            _state = state;
            _culture = culture;
            _defaults = QueryBuilderLabels.From(text);
        }
        return _defaults;
    }
}

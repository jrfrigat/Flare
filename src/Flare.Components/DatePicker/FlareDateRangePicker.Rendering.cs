using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>Selects a date range using two fields or an inline calendar.</summary>
public partial class FlareDateRangePicker
{
    private readonly UnchangedParameters _renderParameters = new();
    private bool _skipParameterRender;
    private CultureInfo? _renderUiCulture;
    private DateOnly _renderToday;

    /// <summary>Applies parameters and skips rendering when their visible output cannot change.</summary>
    /// <param name="parameters">The parameters supplied by the parent.</param>
    /// <returns>The component's parameter lifecycle task.</returns>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        var uiCulture = CultureInfo.CurrentUICulture;
        var today = Today;
        _skipParameterRender = _renderParameters.Matches(parameters, delegatesRender: true)
            && ReferenceEquals(_renderUiCulture, uiCulture) && uiCulture.IsReadOnly && _renderToday == today
            && (!parameters.TryGetValue<DateOnly?>(nameof(StartDate), out var start) || start == StartDate)
            && (!parameters.TryGetValue<DateOnly?>(nameof(EndDate), out var end) || end == EndDate);
        _renderUiCulture = uiCulture;
        _renderToday = today;
        var task = base.SetParametersAsync(parameters);
        _skipParameterRender = false;
        return task;
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if (!_skipParameterRender) return true;
        _skipParameterRender = false;
        return false;
    }
}

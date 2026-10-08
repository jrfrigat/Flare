using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

public partial class FlareMonthGrid
{
    private readonly DayState[] _dayStates = new DayState[42];
    private bool _statesPrepared;
    private bool _skipParameterRender;
    private HeaderState? _headerState;

    private readonly record struct DayState(DateOnly? Day, bool Selected, bool Disabled, string? Css, int Number);
    private sealed record HeaderState(object Weeks, CultureInfo Culture, string? Label, bool Multiple,
        bool WeekNumbers, bool IsoWeeks, DateOnly Today, DateOnly? Cursor, bool Hover, string Week, string WeekNumber);

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        var task = base.SetParametersAsync(parameters);
        // A queued render must not leave a skip or prepared cells for a later local event.
        _skipParameterRender = false;
        _statesPrepared = false;
        return task;
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        if (!_skipParameterRender) return true;
        _skipParameterRender = false;
        return false;
    }

    private void UpdateRenderState()
    {
        var changed = PrepareDayStates();
        var header = new HeaderState(_weeks, _culture, AriaLabel, MultiSelectable, ShowWeekNumbers,
            IsoWeekNumbers, Today, _tabTarget, OnDayHover.HasDelegate, FlareStrings.Picker_Week, FlareStrings.Picker_WeekNumber);
        // Predicates are evaluated before comparison, so a mutable closure still updates the cells.
        // Templates may read arbitrary mutable state and always require a render.
        _skipParameterRender = !changed && header == _headerState && DayContent is null && !_followFocus;
        _headerState = header;
        _statesPrepared = true;
    }

    private void EnsureDayStates()
    {
        if (!_statesPrepared) PrepareDayStates();
        _statesPrepared = false;
    }

    private bool PrepareDayStates()
    {
        var changed = false;
        var index = 0;
        var start = _month.Start;
        var end = _month.End;
        foreach (var week in _weeks)
        {
            foreach (var date in week)
            {
                var state = default(DayState);
                if (date is { } day)
                {
                    var disabled = Disabled?.Invoke(day) ?? false;
                    state = new(day, Selected?.Invoke(day) ?? false, disabled,
                        DayCss(day, day < start || day > end, disabled), DayNumber(day));
                }
                changed |= state != _dayStates[index];
                _dayStates[index++] = state;
            }
        }
        _tabTarget = ResolveTabTarget(_weeks);
        return changed;
    }
}

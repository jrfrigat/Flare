using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Scheduler;

internal static class SchedulerEventKeys
{
    internal static async Task HandleAsync<TItem>(SchedulerEventContext<TItem> context, KeyboardEventArgs e, EventCallback<KeyboardEventArgs> onNavigate = default)
    {
        if (!context.View.CanEdit || e.AltKey || e.MetaKey) return;
        if (e.Key == "F2") { await context.View.Commands.EditEventAsync(context.Occurrence); return; }
        if (e.Key == "Delete") { await context.View.Commands.RequestChangeAsync(context.Occurrence, context.Occurrence.Data, SchedulerEditAction.Delete); return; }
        if (!e.CtrlKey || e.Key is not ("ArrowUp" or "ArrowDown" or "ArrowLeft" or "ArrowRight")) return;
        if (onNavigate.HasDelegate) { await onNavigate.InvokeAsync(e); return; }
        var direction = e.Key is "ArrowUp" or "ArrowLeft" ? -1 : 1;
        var data = context.Occurrence.Data;
        if (data.AllDay is { } days)
        {
            data = data with { AllDay = e.ShiftKey ? days with { End = days.End.AddDays(direction) }
                : new(days.Start.AddDays(direction), days.End.AddDays(direction)) };
        }
        else
        {
            var delta = e.Key is "ArrowLeft" or "ArrowRight" ? TimeSpan.FromDays(direction) : context.View.SlotDuration * direction;
            if (e.Key is "ArrowUp" or "ArrowDown")
            {
                data = data with
                {
                    Start = e.ShiftKey ? data.Start : TimeZoneInfo.ConvertTime(data.Start + delta, context.View.TimeZone),
                    End = TimeZoneInfo.ConvertTime(data.End + delta, context.View.TimeZone)
                };
                context.View.Focus.ReturnFocusTo(context.Occurrence.Identity);
                await context.View.Commands.RequestChangeAsync(context.Occurrence, data, e.ShiftKey ? SchedulerEditAction.Resize : SchedulerEditAction.Move);
                return;
            }
            var wall = TimeZoneInfo.ConvertTime(data.Start, context.View.TimeZone).DateTime.Add(delta);
            var endWall = TimeZoneInfo.ConvertTime(data.End, context.View.TimeZone).DateTime.Add(delta);
            if (context.View.TimeZone.IsInvalidTime(wall) || context.View.TimeZone.IsInvalidTime(endWall)) return;
            DateTimeOffset At(DateTime time, TimeSpan offset) => SchedulerClock.At(DateOnly.FromDateTime(time), TimeOnly.FromDateTime(time), context.View.TimeZone, offset);
            data = e.ShiftKey ? data with { End = At(endWall, data.End.Offset) }
                : data with { Start = At(wall, data.Start.Offset), End = At(endWall, data.End.Offset) };
        }
        context.View.Focus.ReturnFocusTo(context.Occurrence.Identity);
        await context.View.Commands.RequestChangeAsync(context.Occurrence, data, e.ShiftKey ? SchedulerEditAction.Resize : SchedulerEditAction.Move);
    }
}

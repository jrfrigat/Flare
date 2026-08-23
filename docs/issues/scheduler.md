# FlareScheduler: the largest single gap

**Status: OPEN. Phase 2, extra large. Blocks [Gantt](gantt.md).**

This is the biggest hole in Flare's component inventory and the one most likely to lose a head-to-head
evaluation against Radzen.

## Where Flare actually is

`FlareCalendar` is a month grid. Its entire event model is:

```csharp
public sealed record CalendarEvent(string Title, DateOnly Date, FlareColor Color = default);
```

`DateOnly`. No start and end time, no duration, no all-day distinction, no resource, no recurrence. The
component parameters are `Events`, `SelectedDate`, `InitialDate`, `OnEventClick`, `MondayFirst`,
`Culture`, `MonthCount` and `MaxVisibleEvents`. It renders up to three chips in a day cell and a "+N more"
affordance. As a *date* surface that is fine and it should stay. As a scheduler it is not a starting
point - it is a different component.

For comparison: Radzen ships `RadzenScheduler` with day, week, work-week, month, multi-day, agenda, year,
year-planner and year-timeline views. Blazorise ships `Scheduler` with day / week / work-week / month, an
item modal, an occurrence modal and a full recurrence editor including monthly and weekly rule pickers.
MudBlazor and Fluent UI have nothing, so this is Radzen and Blazorise territory alone.

## Scope

Satellite package `Flare.Components.Scheduler`. Not in the core package - it is large, and most
applications do not need it.

### The model comes first

Get this right before rendering anything, because every view is a projection of it:

- `FlareSchedulerEvent`: id, start, end (`DateTimeOffset`, not `DateOnly`), all-day flag, title,
  description, resource id, recurrence rule, exceptions, arbitrary payload via a generic `TItem`.
- **Generic over the application's own type** (`FlareScheduler<TItem>`) with selector parameters, the way
  `FlareDataGrid<T>` works - not a fixed record the caller must map into. This is where Radzen's
  `AppointmentData` sample pattern is weaker and Flare can be better.
- Recurrence as **RFC 5545 RRULE** with `EXDATE` exceptions. Not an invented enum. The expansion is a
  pure function from rule plus window to occurrences, unit-testable with no UI, and it is the part that
  is genuinely hard - budget for it separately.
- Time zones: store `DateTimeOffset`, display in a `TimeZoneInfo` parameter defaulting to local. An
  all-day event is a date range, not a midnight-to-midnight instant - conflating the two is the classic
  scheduler bug and it must be settled in the model.

### Views

`Day`, `Week`, `WorkWeek`, `Month`, `Agenda`, `Timeline` (resource rows against a time axis), and
`Year`. Views are components under a shared `FlareSchedulerViewBase`, so an application can supply its
own view - a first-class extension point, since every industry has one custom view it needs.

The layout engine is one algorithm shared by all time-slot views: given events in a window, produce
lanes for overlaps. Write it once in C#, test it against the nasty cases (identical spans, nested spans,
chains of partial overlaps, zero-duration events), and let every view render its output.

### Interaction

- Click a slot to create; drag an event to move; drag its edge to resize; all raising cancellable events
  so the application owns persistence.
- Keyboard equivalents for all three - move and resize by arrow keys with a modifier. Radzen and
  Blazorise both fall short here; matching them is not the goal.
- Drag between resources in timeline view.
- Editing an occurrence of a recurring series asks "this occurrence / this and following / the whole
  series" - the three-way choice, through `IDialogService`. This is what makes a recurrence engine usable
  and it is where most implementations stop.
- Snapping to a `SlotDuration`, with `MinTime` / `MaxTime` business hours shaded rather than hidden.

### Data

`Events` for the in-memory case and an `EventsProvider` delegate receiving the visible window for the
server case, so a year of data is never shipped to render a week. Virtualize the timeline view's resource
axis.

## Reuse

- `FlareCalendar`'s month-grid geometry and culture handling (first day of week, month names) - extract
  the shared date math rather than duplicating it, and keep `FlareCalendar` rendering the simple case.
- `FlareDataGrid`'s virtualization approach for the resource axis.
- `FlarePopover` and its collision engine for the event peek card - do not write a second positioner.
- `IDialogService` for the editor and the recurrence-scope prompt.
- `FlareResizable`'s pointer handling for the drag-resize gesture, if it generalises; if not, generalise it.
- Kanban's drag model for the move gesture. Note that Kanban uses native HTML5 drag events
  (`ondragstart` / `ondragover`), which **do not fire on touch devices** - the scheduler needs pointer
  events, and this is a good moment to move Kanban onto the same pointer implementation instead of
  maintaining two drag stories.

## Tokens

`SchedulerTokens.cs`, `required`, no literals, and deliberately shared with Gantt and TileLayout per
roadmap rule 3 - all three are timeline-or-cell surfaces. Surface: slot height and border, hour-line and
half-hour-line color and dash, day-column separator, today highlight, weekend and out-of-hours shading,
all-day band background and height, event surface / border / radius / padding / gap, event color roles
(from the chart series palette - a scheduler must not invent its own categorical ramp), selected and
dragging states, drag placeholder and resize handle, now-indicator color and thickness, resource-header
surface, and the month-cell "+N more" affordance.

## Sequencing

Do not attempt this in one branch. Suggested order, each shippable:

1. Model plus RRULE expansion plus the overlap-lane algorithm, with tests and no UI.
2. Day and week views, read-only, with the token record and all three themes.
3. Month and agenda views; converge with `FlareCalendar`'s geometry.
4. Interaction: create, move, resize, keyboard, with cancellable events.
5. Recurrence editing including the three-way scope prompt.
6. Timeline and resource views; year view.
7. `EventsProvider`, virtualization, perf pass.

## Done when

- RRULE expansion matches a published test-vector set including DST boundaries and month-end rules.
- The overlap algorithm has a fixture covering the four degenerate cases listed above.
- Every view renders unstyled without a theme and fully styled with each of the three shipped themes.
- Move and resize are fully keyboard-operable and announced.
- A week of events over a 50k-event source renders from `EventsProvider` without loading the rest.
- No JS beyond existing ports.
- Gallery page per view, plus a recurring-event demo and a resource-timeline demo.

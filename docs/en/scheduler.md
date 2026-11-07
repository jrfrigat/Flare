# Scheduler

Scheduler is a set of calendar UI building blocks over application-owned events. Use the ready-made
composition or arrange independent views, navigation, event actions and editing surfaces yourself.
Month/Year reuse FlareCalendar; Timeline reuses FlareGantt; time-grid gestures reuse the shared
Draggable, DropZone and Resizable. Calendar and Gantt remain independent components.

## Setup and ready-made composition

Install Flare.Components.Scheduler, perform the ordinary Flare/theme setup and register:

```csharp
using Flare.Components.Scheduler;
builder.Services.AddFlareScheduler();
```

Load these stylesheets after the core Flare stylesheet:

```html
<link rel="stylesheet" href="_content/Flare.Components.Gantt/css/gantt.css" />
<link rel="stylesheet" href="_content/Flare.Components.Scheduler/css/scheduler.css" />
```

Use FlareThemeProvider and, for the optional dialog presenter, FlareDialogProvider. Interactive
rendering enables commands and gestures; static server rendering can display data without JS.
Give the active view a bounded parent height when its time grid should scroll.

```razor
@using Flare.Components.Scheduler

<FlareStack Height="36rem">
    <FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData"
                    Events="events" Editable OnChange="Persist" />
</FlareStack>

@code {
    private List<SchedulerEventData> events = [];
    private void Persist(SchedulerChangeEventArgs<SchedulerEventData> change)
    {
        if (change.Original is { } original) events.Remove(original.Item);
        if (change.Action != SchedulerEditAction.Delete) events.Add(change.Data);
    }
}
```

FlareScheduler is a per-instance data/interaction scope. Without ChildContent it composes the public
Surface, Toolbar and Dialog components for convenience. With ChildContent it renders exactly that
composition. Pass visual Class/Style/attributes to the chosen surface in a custom composition.
The scope itself has no dependency on IDialogService.

## Upgrading from 0.52.0

Use the ready-made composition above for a scheduler with the standard views, toolbar and editor.
For a customized scheduler, move the former root parameters to the appropriate components.

| In 0.52.0 | In 0.53.0 |
| --- | --- |
| `ViewTemplate` | Render the view in `FlareScheduler.ChildContent` or replace the view switch with `FlareSchedulerSurface.ChildContent`. |
| `ToolbarTemplate` and `ShowToolbar` | In a custom composition, render your toolbar, add `FlareSchedulerToolbar` for standard commands, or omit the toolbar. |
| `Views` | Set `FlareSchedulerToolbar.Views` to control the view picker. |
| `EditorTemplate` | Put editor content in `FlareSchedulerDialog.ChildContent` or present `context.Edit` in your own layout. See the [drawer example](#own-the-editing-surface). |
| Root `EventTemplate` | Set `EventTemplate` on the surface or an individual view. Use `context.Occurrence` to read the event and `FlareSchedulerEventAction` for its primary action. |
| `AllDayHeight` | Use `AllDayRowHeight` (`--flare-scheduler-all-day-row-height`) to set the minimum height of an all-day lane. |

Selecting a slot updates the selection. Call `context.Commands.CreateEventAsync(slot)` from a
create action when it should open an editing session. Use `OnChange` to persist accepted changes.

## Choose the parts

| Part | Responsibility |
| --- | --- |
| FlareScheduler | Windowed data, navigation, selection, edit requests and persistence boundary |
| SchedulerViewContext / ISchedulerCommands | Loaded snapshot and commands; no concrete visual owner |
| FlareSchedulerSurface | Optional frame, Header/Footer, state templates and one active view |
| FlareSchedulerToolbar | Optional standard commands; Views belongs to this toolbar |
| FlareSchedulerTimeGrid | Day/Week/WorkWeek columns, slots, now line and gestures |
| FlareSchedulerAllDay | Independent date-only lanes and overflow actions |
| FlareSchedulerCalendar | Month/Year using Calendar's grid and headings |
| FlareSchedulerAgenda | Chronological list |
| FlareSchedulerEvent | Shared event interaction, drag and resize wrapper |
| FlareSchedulerEventAction | Primary event selection and keyboard control for custom templates |
| FlareSchedulerTimeline | Virtual resource rows using Gantt's axis and editing |
| SchedulerEditContext | Independent draft, validation and Save/Cancel lifecycle |
| FlareSchedulerEditor | Optional reusable form; no dialog instance required |
| FlareSchedulerDialog | Optional dialog presenter for that same session |

Each view takes Context. A custom host can provide the snapshot and implement ISchedulerCommands;
views do not require a FlareScheduler owner. Surface.ChildContent replaces the view switch.
Header/Footer and LoadingTemplate/ErrorTemplate/EmptyTemplate customize the frame. An empty view
remains selectable unless EmptyTemplate deliberately replaces it. Inactive views are not mounted.

## Own the editing surface

This complete composition puts the same standard form in a right drawer. There is no built-in
editor presenter in a headless composition. Add a sidebar, mini-calendar, application filters,
preview or custom fields alongside these components as needed.

```razor
@using Flare.Components.Scheduler

<FlareScheduler TItem="SchedulerEventData" Mapping="SchedulerMappings.EventData"
                @ref="scheduler" Events="events" Editable OnChange="Persist" OnEventSelected="OpenEvent">
    <ChildContent Context="schedule">
        <FlareStack Height="36rem">
            <FlareSchedulerSurface TItem="SchedulerEventData" Context="schedule">
                <Header Context="navigation">
                    <FlareButton OnClick="navigation.Commands.TodayAsync">Today</FlareButton>
                    <FlareButton OnClick="() => Create(navigation)">New event</FlareButton>
                    <FlareText>@navigation.Caption</FlareText>
                </Header>
            </FlareSchedulerSurface>
        </FlareStack>
        @if (schedule.Edit is { } edit)
        {
            <FlareDrawer Open="true" OpenChanged="open => open ? Task.CompletedTask : edit.CancelAsync()"
                         Anchor="DrawerAnchor.Right" Width="min(30rem,100vw)"
                         CloseOnEsc="@(!edit.IsSaving)" CloseOnScrimClick="@(!edit.IsSaving)"
                         ContentPadding="FlareSpacing.Medium" AriaLabel="Event details">
                <FlareSchedulerEditor TItem="SchedulerEventData" Context="edit" />
            </FlareDrawer>
        }
    </ChildContent>
</FlareScheduler>

@code {
    private FlareScheduler<SchedulerEventData>? scheduler;
    private Task OpenEvent(SchedulerOccurrence<SchedulerEventData> occurrence) => scheduler!.EditEventAsync(occurrence);
    private static Task Create(SchedulerViewContext<SchedulerEventData> context)
    {
        var start = SchedulerClock.At(context.Date, new(9, 0), context.TimeZone);
        return context.Commands.CreateEventAsync(context.SelectedSlot ?? new(start, start.AddHours(1)));
    }
    private List<SchedulerEventData> events = [];
    private void Persist(SchedulerChangeEventArgs<SchedulerEventData> change)
    {
        if (change.Original is { } original) events.Remove(original.Item);
        if (change.Action != SchedulerEditAction.Delete) events.Add(change.Data);
    }
}
```

The built-in labels are localized; the application captions in these small examples are yours to
localize. In a ready-made composition, OnEditRequested transfers presentation to your handler and
suppresses the default Dialog. Keep that session in your own Drawer, Popover, inline form or page.
Commands return after requesting the session, rather than waiting for the user to close it.

Selection does not imply editing. OnEventSelected can open a preview or explicitly call
EditEventAsync. Clicking or pressing Space on a slot only selects it. Double-click/F2 requests
creation; the standard Create button uses the selected interval. Touch users can select and use a
create action. All-day is a property in the common form, not a mandatory per-column create button.

A session holds an immutable Draft record, Original, Action and recurrence Scope. Replace Draft
with a with-expression for custom fields. SaveAsync returns false for invalid/refused/failed/busy
submissions and retains the working copy for retry. Status classifies the failure; Error exposes a
persistence exception. Changed notifies independent presentations. CancelAsync discards the session
without changing the original item. While IsSaving, repeated saves and cancellation are ignored;
keep dismissal disabled until persistence finishes. The default dialog presenter vetoes dismissal
while saving. Accepted saves close the session. Forms can render directly without any presenter.
Editor.ChildContent replaces its fields/actions; using the session directly requires no Editor.

OnChange receives proposed immutable Data, Original/occurrence identity, Action and Scope. Persist
and update the supplied collection or set Cancel to refuse. The library never mutates TItem.
Recurrence moves/resizes and delete requests can ask for scope/confirmation through the same
session. Ordinary moves/resizes submit directly and retain the chosen custom editor workflow.
A refused gesture restores its geometry/focus without opening an editor; OnChangeError reports a
failed gesture. Editor failures stay on their session.

## Event and view composition

EventTemplate on a view/Surface replaces event visuals inside the geometry/drag/resize boundary.
It receives SchedulerEventContext: Occurrence, View, IsSelected, SelectAsync/EditAsync/DeleteAsync
and HandleKeyAsync. Interactive children are supported; no mandatory button surrounds them.
Compose FlareSchedulerEventAction with the template context for the standard primary control and place secondary actions beside it. It uses event typography and caption wrapping; its ChildContent customizes the noninteractive caption.
For other primary controls, bind HandleKeyAsync and data-flare-edit-keys to retain
F2/Delete/modified-arrow shortcuts and native scroll suppression. Do not bind those shortcuts to
nested fields or secondary menus. Focus restoration uses the first enabled interactive child.

The default event button supports F2, Delete, Ctrl+arrows (move) and Ctrl+Shift+arrows (resize).
Timeline navigation reuses Gantt's axis step and resource commands. Time slots use arrows,
Home/End and Space, including RTL direction and repeated DST times. Custom visuals must fit the
allocated timed geometry; keep large details in a separate presentation.

TimeGrid exposes DayHeaderTemplate, noninteractive SlotTemplate, ShowAllDay and AllDayTemplate.
The latter receives SchedulerTimeGridContext with View and the exact visible Dates, including
work-week filtering. Calendar exposes month, weekday and date headings. Its Scheduler events use
Calendar.EventContainerTemplate rather than placing nested actions in Calendar's default control.
Timeline uses Gantt.ItemInteractionTemplate, whose context preserves the standard item/axis commands.
Timeline also exposes Scale, UnitWidth, RowHeight and RowHeaderTemplate. Its default virtualized mode needs viewport measurement; set Virtualize=false for a small static server-rendered projection.
Mark secondary bar actions with data-gantt-no-drag; input fields retain native editing behavior.

## All-day lanes and overflow

```razor
<FlareSchedulerTimeGrid TItem="SchedulerEventData" Context="schedule">
    <AllDayTemplate Context="band">
        <FlareSchedulerAllDay TItem="SchedulerEventData" Context="band.View" Dates="band.Dates"
                             MaxRows="3" />
    </AllDayTemplate>
</FlareSchedulerTimeGrid>
```

The shared band aligns to the timed columns and draws a multi-day event as one clipped segment.
Touching exclusive ends share a lane. By default it grows without an internal scrollbar. MaxRows
adds +N controls; default actions expand/collapse the band. Expanded/ExpandedChanged support
controlled expansion. OnOverflow receives the date and hidden occurrences for your Drawer,
Popover or Agenda; its ShowAsync can still expand the band. OverflowTemplate replaces the action.
HeaderTemplate/SlotTemplate/SegmentTemplate customize the independent band. A complete custom
segment owns interaction/drag/keyboard behavior; EventTemplate retains the standard wrappers.
ScrollOverflow plus MaxHeight opts into one scroll area for the whole band. Disable TimeGrid's
ShowAllDay to place an independent band elsewhere. AllDayRowHeight is a minimum, not a fixed
height that clips larger text.

## Data, culture and themes

Map your own model through SchedulerMapping<TItem>; no inheritance is required. Stable keys
identify events. Timed intervals use instant Start/End; all-day SchedulerDateRange uses independent
DateOnly bounds. Every End is exclusive. Resources identify timeline rows, not calendars. Keep
CalendarId, permissions and business fields in your application model and filter Events/provider
requests there. Preserve those fields when applying move/resize proposals.

Date/DateChanged and View/ViewChanged support controlled state; without callbacks navigation is
internal. Culture controls formatting and the first weekday. UI text uses the scoped
[localization](localization.md) contract and standard IStringLocalizer<SchedulerStrings> overrides.
Browser language switching preserves the session, draft and focus. An explicit Culture remains
authoritative. TimeZone changes instant display, never all-day dates. Missing DST slots are omitted;
repeated wall times retain their distinct offsets. InitialScrollTime defaults to WorkStart.

EventsProvider receives the half-open Window and a cancellation token. Include intersecting events,
including those beginning earlier. Stale results are ignored even if cancellation is ignored.
ReloadAsync refreshes external changes; OnLoadError and Surface.ErrorTemplate handle load errors.
Windowed snapshots and cached day/slot/lane indexes avoid rescanning source data on clock/selection
renders. Set MaximumOccurrences/MaximumEvaluationSteps to bound local work.

Ready backend occurrences work without an RFC dependency. Stable SeriesKey/RecurrenceId retain
series identity. Flare.Components.Scheduler.Ical is an optional local RRULE/RDATE/EXDATE adapter;
explicitly set RecurrenceExpander to IcalSchedulerRecurrenceExpander.Default. Storage, recurrence
rules, series splitting, invitations and application workflows remain application/backend concerns.

Scheduler tokens default to semantic base tokens. Override SchedulerTokens with [CssVar] and the
common mapper, just like other independent packages:

```csharp
using Flare.Theming;
using Flare.Components.Scheduler;
using static Flare.Css.Tokens.Vars;

var values = new SchedulerTokens
{
    EventSurface = Var(Flare.Css.Tokens.Color.TertiaryContainer),
    EventColor = Var(Flare.Css.Tokens.Color.OnTertiaryContainer)
}.FlattenTokens();
themeService.SetCustomTokens(values);
```

FlattenTokens includes the record's defaults: filter keys when applying a partial override over
existing customizations. Themes can use DesignTokens.Extended. CSS inheritance scopes consumer
styles; ClearCustomToken removes a service override. Public CSS class constants identify the parts.

The [Gallery](https://jrfrigat.github.io/Flare/components/scheduler) shows the ready composition,
calendar sidebar/filters/preview/right drawer, quick/full inline forms with business fields,
external all-day overflow, windowed data and optional recurrence.

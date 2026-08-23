# FlareGantt: project timeline on the Scheduler's engine

**Status: OPEN. Phase 2, large. Blocked by [Scheduler](scheduler.md) - do not start before its timeline
engine exists.**

Radzen has `RadzenGantt` with day / week / month / year / years views; Blazorise has `Gantt` with a tree
pane, a timeline pane, item bars, a progress editor and an item modal. MudBlazor and Fluent UI have
nothing. Flare has `FlareTimeline` (a vertical narrative list, unrelated) and `FlareKanban` (board, not
schedule).

The reason this issue is *after* the Scheduler rather than beside it: a Gantt chart is a resource
timeline whose rows are a task tree and whose bars carry dependencies. Eighty percent of it - the time
axis, the zoom levels, the drag-move and drag-resize gestures, the virtualized row axis, the token
family - is the Scheduler's timeline view. Building them separately would produce two of everything and
would violate roadmap rule 3 twice over.

## Scope

Satellite package `Flare.Components.Gantt`, referencing `Flare.Components.Scheduler` for the timeline
engine. If that reference feels wrong, the engine is in the wrong place - extract it to a shared internal
package rather than duplicating it.

What Gantt adds over the timeline view:

1. **A task tree in the left pane**, expandable, with columns. This is `FlareDataTree` plus
   `FlareDataGrid` columns - reuse both, do not write a third tree. The two panes are `FlareSplitter`
   with synchronised vertical scrolling.
2. **Summary tasks** whose bar spans and whose progress rolls up from children.
3. **Dependencies** - finish-to-start, start-to-start, finish-to-finish, start-to-finish - drawn as
   routed connectors between bars, created by dragging from a bar's edge handle. Connector routing is
   SVG path math in C#, no JS.
4. **Constraint behaviour on drag**: moving a predecessor either pushes dependents or does not, per a
   `ScheduleMode` parameter. Raise the computed change set as a cancellable event; never mutate the
   caller's data.
5. **Progress editing** by dragging the fill within a bar.
6. **Milestones** as zero-duration diamond markers.
7. **Baselines** - a second, muted bar per task showing the planned span against the actual. Neither
   Radzen nor Blazorise has this, and it is what project managers actually look at.
8. **Critical path** as an optional computed highlight - forward and backward pass over the dependency
   graph, a pure function, unit-testable, and a genuine differentiator.
9. **Non-working time** shading from a calendar (weekends, holidays), with an option for durations to be
   measured in working days.
10. **Zoom levels** - hour / day / week / month / quarter / year - sharing the Scheduler's axis renderer.

## What to be careful about

- **Row virtualization is mandatory**, not optional. A real project plan is thousands of rows, and both
  panes must virtualize in lockstep or the panes desynchronise on fast scroll.
- **Dependency drawing is the perf trap.** Connectors must be computed from the visible window only, and
  recomputed on scroll without re-laying-out the bars.
- **Cycle detection** on dependency creation, refused with a localized message rather than an exception.
- Dates are `DateTimeOffset` and durations are working-time aware - the same model decision the Scheduler
  settles, inherited rather than re-litigated.

## Tokens

Extends the Scheduler token family; adds only what is Gantt-specific, `required`, no literals: bar
surface / border / radius / height, summary-bar shape, milestone glyph size and shape, progress-fill
color and opacity, baseline bar color and offset, dependency connector color / width / dash / arrowhead
size, critical-path emphasis, non-working shading, and the tree-pane / timeline-pane divider (reuse
`SplitterTokens`).

## Done when

- A 5000-task plan with 8000 dependencies scrolls smoothly with both panes in sync.
- Dragging a predecessor produces the correct dependent shift for all four dependency types, proven by a
  fixture, and the change set is cancellable.
- Critical path matches a hand-computed reference plan.
- Cycles are refused, not thrown.
- Keyboard: move, resize and re-parent a task without a pointer.
- No JS beyond the existing ports; connectors are SVG.
- Gallery page with a realistic plan, baselines on, critical path toggle.

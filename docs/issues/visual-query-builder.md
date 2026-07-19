# A standalone visual query builder that emits a backend-agnostic query spec

**Source:** Weir. Its admin needs ad-hoc historical analytics over the request log - an overall view and
a per-endpoint view, aggregated over a chosen window (24h / 7d / 30d): counts, error rate, cache-hit
ratio, latency percentiles, grouped by route or by day. Building a bespoke filter+aggregate UI, and a
hand-written `GROUP BY` per database provider (Weir ships SQLite, PostgreSQL and SQL Server), is the
wrong layer to solve this at. The reusable half - "let a user compose a query visually" - belongs in
Flare; only the "run it against my store" half belongs in the app.

**Severity:** medium. Nothing is blocked (Weir can hand-roll one), but every consumer that wants ad-hoc
filtering + aggregation over tabular data will otherwise rebuild the same widget, which is exactly the
duplication Flare exists to prevent.

## The core principle: separate building from executing

The component **builds a query; it never runs one.** Its output is a **serializable, backend-agnostic
query specification** - a plain data object. What executes it lives one level up, in the consumer:
Weir translates the spec to raw SQL for whichever provider is configured; an EF app turns it into an
`IQueryable`; an HTTP client posts it to an API. The builder must know nothing about any of that.

This is the one thing today's Flare filter model does *not* give, and it is the whole point (see below).

## What Flare already has (and why it is not enough)

The DataGrid carries a real, recursive filter model that this should build on, not replace:

- `FilterOperator` (`Contains`, `Equals`, `GreaterThan`, `Between`, ...) - the vocabulary is already right.
- `FilterCondition` (field + operator + value(s)) and `FilterGroupNode` (nested `Conditions` + child
  `Groups` + an `Or` flag) - a proper nested AND/OR tree.
- `DataGridRequest` (filters + sorts + paging) and `DataGridQuery.Execute<T>(IQueryable<T>, request)`.

Three gaps make it unusable for the case above:

1. **It is bolted to the grid.** The filter UI is a per-column menu; there is no standalone builder a
   page can drop in to compose a whole query outside a grid.
2. **Its execution is `IQueryable`/EF-coupled.** `DataGridQuery.Execute` runs the request against an
   `IQueryable` (EF Core turns it into SQL). Weir's control-plane store is raw ADO/Dapper across three
   providers - there is no `IQueryable` to hand it. The **spec must be decoupled from execution** so a
   non-EF consumer can translate it itself.
3. **No aggregation or grouping.** The model filters, sorts and pages rows. Analytics needs
   `GROUP BY` + aggregates (count, sum, avg, min, max, and ideally percentiles), which the model has no
   vocabulary for.

## What is requested

1. **A standalone `FlareQueryBuilder` component.** Not inside a grid. It takes a **field schema** and
   raises the composed query on change / submit.
2. **A field schema as input** - the fields the user may query, each with a display label, a data type
   (text / number / bool / date-time / enum with its members), the operators allowed for it, and flags
   for whether it is groupable and which aggregates it permits. The consumer supplies this; it is the
   only place the builder learns about the data.
3. **A backend-agnostic query spec as output**, serializable to JSON, carrying:
   - **Select / aggregates**: fields to return and aggregates to compute (`Count`, `Sum`, `Avg`,
     `Min`, `Max`, and percentiles such as `P50`/`P95`/`P99` where the backend can do them).
   - **Filter**: reuse the existing `FilterGroupNode` / `FilterCondition` / `FilterOperator` tree
     verbatim, so a query built here and a grid's column filters speak one language.
   - **Group by**: zero or more fields (including a date-truncation option - by day / hour - for time
     series).
   - **Sort** and **limit**.
   - Optionally a **named time-range** helper on top of a date field (last 24h / 7d / 30d / custom), since
     that is the near-universal first filter.
   The component **only** emits this object (an `OnQueryChanged` / `OnSubmit` callback). It issues no
   request and holds no data source.
4. **Reuse, do not fork, the filter types.** Extending `FilterGroupNode` with the select/group/aggregate
   parts (or wrapping it in a new `FlareQuerySpec`) keeps the grid and the builder interoperable.

## Consumer contract (what stays in the app)

The consumer receives the spec and does whatever its store needs:

- An EF app can keep using `IQueryable` - Flare could optionally ship a spec-to-`IQueryable` translator
  (the aggregate-aware successor to `DataGridQuery.Execute`) as a convenience, but it must be opt-in and
  separate from the builder.
- Weir translates the spec to parameterized SQL per provider and renders the result set into a
  `FlareDataGrid` (and a `FlareChart` for a trend). The builder does not know SQL exists.

## Weir's concrete use

Fields from the request log: `route`, `timestamp`, `durationMs`, `cacheHit`, `error`, `status`,
`apiKeyPrefix`. A user composes: filter `timestamp` in the last 30 days and `error = true`; aggregate
`count`, error rate, `P50`/`P95` of `durationMs`; group by `route` (or by day for a trend). The same
builder, with the filter preset to one route, is the per-endpoint drill-down. "Overall" and
"per-endpoint" stop being two features and become one query with a different filter.

Until this exists, Weir will either hand-roll a narrow aggregation UI + per-provider SQL for the request
log, or defer historical analytics entirely and point operators at an OTLP / Prometheus export for
long-term and cross-instance history (which stays the right answer for fleet-wide metrics regardless).

# Flare.Components.Query

Visual query designer for the [Flare](https://github.com/jrfrigat/Flare) Blazor component library,
built on the Querio query model. Add-on package that extends `Flare.Components`.

```sh
dotnet add package Flare.Components.Query
```

Requires `Flare.Components` and a `Flare.Theme.*` package. Use `<FlareQueryBuilder ... />` or
`<FlareQueryEditor ... />` once Flare is set up (see the `Flare.Components` readme).

Composes joins, aggregates, grouping, conditions and paging over a schema the caller supplies, and
emits a serializable query spec. It does not connect to a database and does not execute anything: what
the spec becomes - SQL, an HTTP request, a LINQ expression - is the consumer's decision.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.

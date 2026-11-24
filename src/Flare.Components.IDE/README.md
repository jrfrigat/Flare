# Flare.Components.IDE

IDE-style layout components for the [Flare](https://github.com/jrfrigat/Flare) Blazor component
library - ribbon, document tabs, tool panels, splitter and status bar. Add-on package that extends
`Flare.Components`.

```sh
dotnet add package Flare.Components.IDE
```

Requires `Flare.Components` and a `Flare.Theme.*` package. Register with
`builder.Services.AddFlareIde()` (see the `Flare.Components` readme for base setup).

## Dynamic docking

Use `FlareDockWorkspace` to make any Blazor content dockable. Supply a `DockGroup` for
the initial tabs or a tree of `DockSplit` and `DockGroup` nodes. Reference
`_content/Flare.Components.IDE/css/flare-ide.css` and give the workspace room to expand.

```razor
@using Flare.Components.IDE

<FlareDockWorkspace Layout="_layout" Items="_items" OnChangeRequested="Apply">
    <ContentTemplate Context="item">
        <FlareText>@item.Title</FlareText>
    </ContentTemplate>
</FlareDockWorkspace>

@code {
    private DockNode _layout = new DockGroup("main", ["data", "report"], "data");
    private readonly DockItem[] _items = [new("data", "Data"), new("report", "Report")];

    private void Apply(DockLayoutChange change)
    {
        if (ReferenceEquals(change.Previous, _layout))
            _layout = change.Proposed;
    }
}
```

Drag a tab to a group edge to create a split. The tab actions menu also supports moving,
reordering, and splitting by click, tap, or keyboard. Each group keeps its own selection.
Use unique item and node keys. A group can be empty when it is the root.

Set `AdaptiveTabCompression="true"` to fit tabs to each group. It is off by default.
As a group narrows, tab labels truncate, then give way to the icons supplied in `DockItem.Icon`.
Tabs without an icon keep a short label and every tab keeps `DockItem.Title` as its accessible name.
The scroll arrows appear when the compact tabs need more room. Widen the group to restore the labels.
With a custom `TabTemplate`, supply `DockItem.Icon` and `DockItem.Title` for the compact view.
Use `TabMarkerTemplate` for indicators that should stay visible and `TabActionsTemplate` for tab controls.

The application owns the accepted layout. It can reject a request by keeping its current
layout. After an asynchronous confirmation, check that the request's `Previous` layout
is still current. Save your own layout representation and application data as needed.

An active item retains its component when it moves within the same workspace and renderer.
Use `DockItem.KeepAlive` to retain a visited item when its tab becomes inactive. Closing
releases the component. The application can restore its data when it opens the item again.

Repository & docs: https://github.com/jrfrigat/Flare  -  MIT licensed.

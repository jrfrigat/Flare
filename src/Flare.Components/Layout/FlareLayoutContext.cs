namespace Flare.Components;

/// <summary>
/// Cascading coordinator shared by <see cref="FlareLayout"/> and the components placed inside it
/// (<see cref="FlareLayoutAppBar"/>, <see cref="FlareLayoutDrawer"/>, <see cref="FlareLayoutContent"/>).
/// Drawers register themselves here; the layout reads the registry to reserve a grid track for each
/// in-flow drawer and to drive the app-bar toggle and the mobile scrim. Each drawer owns its own
/// open state -- this context only aggregates them.
/// </summary>
public sealed class FlareLayoutContext
{
    private readonly List<FlareLayoutDrawer> _drawers = new();
    private bool _isMobile;

    /// <summary>Raised when the registry, a drawer's state, or the breakpoint changes.</summary>
    public event Action? StateChanged;

    /// <summary>
    /// Whether the layout is below its mobile breakpoint. Push drawers (Rail/Persistent/Responsive)
    /// become off-canvas overlays while true. Set by <see cref="FlareLayout"/> from the breakpoint
    /// watcher; setting it notifies subscribers.
    /// </summary>
    public bool IsMobile
    {
        get => _isMobile;
        set
        {
            if (_isMobile == value) return;
            _isMobile = value;
            StateChanged?.Invoke();
        }
    }

    /// <summary>Registers a drawer (called on its initialization) so it contributes a layout track.</summary>
    public void Register(FlareLayoutDrawer drawer)
    {
        if (_drawers.Contains(drawer)) return;
        _drawers.Add(drawer);
        StateChanged?.Invoke();
    }

    /// <summary>Removes a drawer from the registry (called on its disposal).</summary>
    public void Unregister(FlareLayoutDrawer drawer)
    {
        if (_drawers.Remove(drawer)) StateChanged?.Invoke();
    }

    /// <summary>Notifies the layout that a registered drawer changed its open/variant state.</summary>
    public void NotifyStateChanged() => StateChanged?.Invoke();

    /// <summary>
    /// The grid track list for the layout body: one track per in-flow drawer (in markup order),
    /// with a flexible content track in between the start and end drawers. Overlay/off-canvas drawers
    /// are out of flow and contribute no track.
    /// </summary>
    public string GridTemplateColumns
    {
        get
        {
            var start = _drawers
                .Where(d => d.Anchor != DrawerAnchor.Right && d.ReservesTrack(_isMobile))
                .Select(d => d.TrackWidth(_isMobile));
            var end = _drawers
                .Where(d => d.Anchor == DrawerAnchor.Right && d.ReservesTrack(_isMobile))
                .Select(d => d.TrackWidth(_isMobile));
            return string.Join(" ", start.Append("minmax(0, 1fr)").Concat(end));
        }
    }

    /// <summary>
    /// The drawer the app-bar toggle controls: the first non-temporary start drawer (the primary
    /// navigation). Null when the layout has no such drawer.
    /// </summary>
    public FlareLayoutDrawer? PrimaryDrawer =>
        _drawers.FirstOrDefault(d => d.Anchor != DrawerAnchor.Right && d.Variant != DrawerVariant.Temporary);

    /// <summary>Whether the primary drawer is currently open (drives the app-bar toggle's pressed state).</summary>
    public bool PrimaryOpen => PrimaryDrawer?.IsOpen ?? false;

    /// <summary>Toggles the primary drawer open/closed. No-op when there is no primary drawer.</summary>
    public Task TogglePrimaryAsync() => PrimaryDrawer?.ToggleAsync() ?? Task.CompletedTask;

    /// <summary>True when any drawer is currently shown as a floating overlay (so the scrim is active).</summary>
    public bool AnyOverlayOpen => _drawers.Any(d => d.IsOverlayOpen(_isMobile));

    /// <summary>Closes every drawer that is currently a floating overlay (e.g. on scrim tap).</summary>
    public async Task CloseOverlaysAsync()
    {
        foreach (var drawer in _drawers.Where(d => d.IsOverlayOpen(_isMobile)).ToList())
            await drawer.SetOpenAsync(false);
    }
}

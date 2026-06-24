namespace Flare.Components;

/// <summary>
/// Cascading context passed from FlareTabs to FlareTab children with IsFixed="true".
/// Carries a snapshot of the active tab and a callback to activate a tab.
/// NOTE: Because tabs register themselves by object reference, the context is rebuilt
/// whenever the active tab changes so children receive an updated snapshot.
/// </summary>
internal sealed record FlareTabsContext(
    FlareTab? ActiveTab,
    Action<FlareTab> Activate,
    Action<FlareTab> Register,
    Action<FlareTab> Unregister,
    bool Lazy = false);

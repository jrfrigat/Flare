namespace Flare.Components;

/// <summary>
/// Anti-flash bootstrap for server-rendered and prerendered apps: a synchronous script in the document
/// head that applies the visitor's saved theme, palette and mode before the first paint, so the page
/// never appears in the default theme and then switches.
/// </summary>
/// <remarks>
/// A component renders after Blazor has started, which is already too late for a purely static
/// WebAssembly app - paste the generated script into <c>index.html</c> there instead.
/// </remarks>
public partial class FlareBootstrap;

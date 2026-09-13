namespace Flare.Components;

/// <summary>Who writes the active theme's stylesheet links into the document head.</summary>
public enum ThemeStylesheets
{
    /// <summary>
    /// <c>FlareThemeProvider</c> emits a link for every registered theme's <c>StyleAssets</c> and makes
    /// sure the active theme's sheets have loaded before it reveals the app. Nothing to write by hand,
    /// but in a WebAssembly app the links reach the page only once .NET has started.
    /// </summary>
    Automatic,

    /// <summary>
    /// The application writes the theme's <c>&lt;link rel="stylesheet"&gt;</c> tags into its own head, so
    /// the browser fetches them with the first wave of the page instead of after .NET starts. The
    /// provider emits no links of its own; it still waits for the active theme's sheets and the active
    /// palette's <c>StyleAsset</c>, reuses a link the page already has for the same address, and adds only
    /// a sheet that is missing, such as one for a theme or palette the user switches to at run time.
    /// </summary>
    Manual,
}

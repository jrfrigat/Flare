namespace Flare.Components;

/// <summary>
/// The root every Flare app is wrapped in: it cascades the active theme to every component below,
/// restores the visitor's saved theme, palette and mode, follows the OS light/dark preference and its
/// accent colour, and holds the app's splash until the themed first frame has painted.
/// </summary>
public partial class FlareThemeProvider;

using Flare.Abstractions;

namespace Flare.Gallery.Services;

/// <summary>
/// Gallery UX helper: in Flare a theme is NOT bound to a palette (any theme runs with any
/// palette). So when the user switches the design system without having explicitly picked a
/// palette, follow the new theme's OWN default palette - e.g. selecting Fluent UI 2 shows Fluent
/// blue rather than whatever palette happened to be active. Once the user explicitly picks a
/// palette, that choice is "pinned" and survives further theme switches until they change it again.
///
/// Implemented centrally by observing <see cref="IThemeService.OnThemeChanged"/> (which fires for
/// every theme/palette/mode change) and inferring intent from what actually changed. A re-entrancy
/// guard prevents the follower's own palette switch from looping.
/// </summary>
public sealed class ThemePaletteFollower
{
    private readonly IThemeService _theme;
    private string _lastTheme;
    private string _lastPalette;
    private bool _pinned;
    private bool _reentrant;

    public ThemePaletteFollower(IThemeService theme)
    {
        _theme = theme;
        _lastTheme = theme.CurrentTheme.Id;
        _lastPalette = theme.CurrentPalette.Id;
        theme.OnThemeChanged += OnThemeChangedAsync;
    }

    private async Task OnThemeChangedAsync()
    {
        if (_reentrant) return;

        var themeId = _theme.CurrentTheme.Id;
        var paletteId = _theme.CurrentPalette.Id;

        if (themeId != _lastTheme)
        {
            // Design system changed. Follow this theme's default palette unless the user pinned one.
            _lastTheme = themeId;
            var wanted = _theme.CurrentTheme.DefaultPaletteId;
            if (!_pinned && !string.IsNullOrEmpty(wanted) && paletteId != wanted
                && _theme.Palettes.Any(p => p.Id == wanted))
            {
                _reentrant = true;
                try { await _theme.SetPaletteAsync(wanted); }
                finally { _reentrant = false; }
                _lastPalette = _theme.CurrentPalette.Id;
                return;
            }
        }
        else if (paletteId != _lastPalette)
        {
            // Palette changed without a theme change => an explicit user choice. Pin it.
            _pinned = true;
        }

        _lastPalette = paletteId;
    }
}

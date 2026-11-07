using Flare.Abstractions;

namespace Flare.Gallery.Services;

/// <summary>Restores only the custom-token keys a Gallery preview changed.</summary>
public sealed class ThemeTokenPreview(IThemeService theme) : IDisposable
{
    private readonly Dictionary<string, string?> _previous = new(StringComparer.Ordinal);
    /// <summary>Applies preview values, keeping the original value of each affected key.</summary>
    public void Apply(IReadOnlyDictionary<string, string> values)
    {
        var current = theme.GetCustomTokens();
        foreach (var key in values.Keys)
            _previous.TryAdd(key, current.TryGetValue(key, out var value) ? value : null);
        theme.SetCustomTokens(values);
    }
    /// <summary>Restores the original values and leaves unrelated customization alone.</summary>
    public void Dispose()
    {
        var restore = new Dictionary<string, string>();
        foreach (var (key, value) in _previous)
            if (value is null) theme.ClearCustomToken(key);
            else restore[key] = value;
        if (restore.Count > 0) theme.SetCustomTokens(restore);
        _previous.Clear();
    }
}

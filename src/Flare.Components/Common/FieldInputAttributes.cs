namespace Flare.Components;

/// <summary>
/// Consumer-supplied <c>InputAttributes</c> shared by the field family. <see cref="Id"/> resolves the
/// DOM id a field's label and validation-summary link must reference; <see cref="Forwardable"/> filters
/// the attributes a select shell allows onto its active control, because the shell owns the combobox
/// role, its listbox wiring, the value and the state ARIA.
/// </summary>
internal static class FieldInputAttributes
{
    internal static string Id(IReadOnlyDictionary<string, object>? attributes, string fallback)
    {
        if (attributes is not null)
            foreach (var (name, value) in attributes)
                if (name.Equals("id", StringComparison.OrdinalIgnoreCase)) return value?.ToString() ?? fallback;
        return fallback;
    }

    private static readonly HashSet<string> _owned = new(StringComparer.OrdinalIgnoreCase)
    {
        "role", "aria-haspopup", "aria-expanded", "aria-controls", "aria-activedescendant",
        "aria-autocomplete", "aria-required", "aria-invalid", "value",
    };

    internal static IReadOnlyDictionary<string, object>? Forwardable(IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes is null || attributes.Count == 0) return attributes;
        var filtered = false;
        foreach (var name in attributes.Keys)
        {
            if (IsOwned(name)) { filtered = true; break; }
        }
        if (!filtered) return attributes;

        var copy = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var (name, value) in attributes)
        {
            if (!IsOwned(name)) copy[name] = value;
        }
        return copy;
    }

    // Event handlers are owned too: a splatted handler would replace the one that opens and filters the widget.
    private static bool IsOwned(string name) =>
        _owned.Contains(name) || name.StartsWith("on", StringComparison.OrdinalIgnoreCase);
}

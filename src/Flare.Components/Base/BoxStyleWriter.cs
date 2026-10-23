using System.Text;

namespace Flare.Components;

/// <summary>
/// Builds the inline declarations of the box and spacing parameters. Nothing is allocated until a
/// parameter is set, so the common case - a box or a text run with none of them - costs no string.
/// </summary>
internal struct BoxStyleWriter
{
    private StringBuilder? _style;

    /// <summary>Writes a spacing declaration from a scale step, or from the raw value of a
    /// <see cref="FlareSpacing.Custom"/> step.</summary>
    public void Spacing(string property, FlareSpacing? step, string? raw)
    {
        if (ResolveSpacing(step, raw) is { } value) Write(property, value);
    }

    /// <summary>Writes a declaration from a consumer value, sanitized; an empty value writes nothing.</summary>
    public void Value(string property, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var safe = Security.CssValidator.StripDangerous(value);
        if (!string.IsNullOrWhiteSpace(safe)) Write(property, safe);
    }

    /// <summary>Writes a declaration whose value the component produced itself.</summary>
    public void Write(string property, string value)
    {
        _style ??= new StringBuilder(64);
        if (_style.Length > 0) _style.Append(';');
        _style.Append(property).Append(':').Append(value);
    }

    /// <summary>The declarations written so far, or null when none was.</summary>
    public readonly string? Build() => _style?.ToString();

    /// <summary>The CSS value of a spacing step, or the sanitized raw value of a custom one.</summary>
    public static string? ResolveSpacing(FlareSpacing? step, string? raw) => step switch
    {
        null => null,
        FlareSpacing.Custom => Security.CssValidator.StripDangerous(raw),
        _ => FlareSpacingVars.Var(step.Value),
    };

    /// <summary>Joins two style fragments with ';', skipping an empty one, without an array.</summary>
    public static string? Join(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first)) return string.IsNullOrWhiteSpace(second) ? null : second;
        return string.IsNullOrWhiteSpace(second) ? first : string.Concat(first, ";", second);
    }

    /// <summary>Joins style fragments in order with ';', skipping the empty ones.</summary>
    public static string? Join(params string?[] parts)
    {
        string? single = null;
        StringBuilder? joined = null;
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;
            if (single is null && joined is null)
            {
                single = part;
                continue;
            }

            joined ??= new StringBuilder(single);
            joined.Append(';').Append(part);
        }

        return joined?.ToString() ?? single;
    }
}

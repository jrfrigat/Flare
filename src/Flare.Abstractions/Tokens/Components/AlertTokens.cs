namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme token values for <c>FlareAlert</c>.</summary>
public sealed record AlertTokens
{
    /// <summary>Border radius of the alert container.</summary>
    public string Radius { get; init; } = "0.5rem";

    /// <summary>Border width for outlined and text variants (filled uses 0).</summary>
    public string BorderWidth { get; init; } = "1px";

    /// <summary>Internal padding of the alert container.</summary>
    public string Padding { get; init; } = "0.875rem 1rem";

    /// <summary>Gap between icon and content.</summary>
    public string Gap { get; init; } = "0.75rem";
}

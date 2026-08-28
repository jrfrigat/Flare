namespace Flare.Components;

/// <summary>
/// Card width below which <c>FlareCardActions</c> stacks its actions. These are CONTAINER widths, not
/// viewport breakpoints: what decides whether two buttons fit side by side is how wide the card is, so a
/// card in a narrow column stacks on a wide screen too.
/// </summary>
public enum CardActionsStack
{
    /// <summary>Never stack automatically (the default).</summary>
    Never,
    /// <summary>Stack below 20rem (320px) - only genuinely cramped cards.</summary>
    Narrow,
    /// <summary>Stack below 30rem (480px) - the phone-width card.</summary>
    Compact,
    /// <summary>Stack below 40rem (640px) - anything short of a full-width card.</summary>
    Wide,
}

namespace Flare.Components;

/// <summary>Alignment of <c>FlareCardActions</c> content along the row (or column, when vertical).</summary>
public enum CardActionsAlign
{
    /// <summary>Align actions to the start (left in LTR).</summary>
    Start,
    /// <summary>Center the actions.</summary>
    Center,
    /// <summary>Align actions to the end (right in LTR).</summary>
    End,
    /// <summary>Distribute actions with space between them.</summary>
    Between,
    /// <summary>Stretch the actions to fill the row, sharing the width equally.</summary>
    Stretch,
}

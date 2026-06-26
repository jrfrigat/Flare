using Microsoft.AspNetCore.Components;

namespace Flare.Abstractions;

/// <summary>
/// Provides collision detection for popovers, tooltips, and menus.
/// Calculates the best placement to avoid viewport overflow.
/// </summary>
public interface ICollisionService : IAsyncDisposable
{
    /// <summary>
    /// Calculate the best placement for a floating element.
    /// </summary>
    ValueTask<CollisionResult> CalculatePlacementAsync(
        ElementReference anchor,
        ElementReference floating,
        string preferredPlacement,
        CollisionOptions? options = null,
        CancellationToken ct = default);
}

/// <summary>Options for collision detection.</summary>
public sealed class CollisionOptions
{
    /// <summary>Distance between anchor and floating element in pixels.</summary>
    public int Offset { get; set; } = 8;
    /// <summary>Enable automatic flip when space is insufficient.</summary>
    public bool Flip { get; set; } = true;
    /// <summary>Enable shifting to keep within viewport.</summary>
    public bool Shift { get; set; } = true;
    /// <summary>Size of the arrow element in pixels.</summary>
    public int ArrowSize { get; set; }
    /// <summary>Padding from viewport edges.</summary>
    public int BoundaryPadding { get; set; }
}

/// <summary>Result of collision detection calculation.</summary>
public sealed class CollisionResult
{
    /// <summary>Final placement (may differ from preferred if flipped).</summary>
    public string Placement { get; init; } = "";
    /// <summary>Calculated top position in pixels.</summary>
    public double Top { get; init; }
    /// <summary>Calculated left position in pixels.</summary>
    public double Left { get; init; }
    /// <summary>Arrow top position (relative to floating element).</summary>
    public double ArrowTop { get; init; }
    /// <summary>Arrow left position (relative to floating element).</summary>
    public double ArrowLeft { get; init; }
    /// <summary>Whether the placement was flipped from the preferred.</summary>
    public bool NeedsFlip { get; init; }
}

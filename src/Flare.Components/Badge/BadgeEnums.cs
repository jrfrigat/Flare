namespace Flare.Components;

/// <summary>Corner position of the badge indicator relative to its anchor element.</summary>
public enum BadgeOrigin
{
    /// <summary>Top right.</summary>
    TopRight,
    /// <summary>Top left.</summary>
    TopLeft,
    /// <summary>Bottom right.</summary>
    BottomRight,
    /// <summary>Bottom left.</summary>
    BottomLeft,
}

/// <summary>Shape of the anchor element that determines how the badge indicator overlaps its edge.</summary>
public enum BadgeOverlap
{
    /// <summary>Anchor element has a circular shape (e.g. avatar); badge is positioned at the curved edge.</summary>
    Circular,
    /// <summary>Anchor element has a rectangular shape; badge is positioned at the corner.</summary>
    Rectangular,
}

/// <summary>Corner of the anchor element where the badge indicator is placed.</summary>
public enum BadgeAnchor
{
    /// <summary>Top-right corner of the anchor.</summary>
    TopRight,
    /// <summary>Top-left corner of the anchor.</summary>
    TopLeft,
    /// <summary>Bottom-right corner of the anchor.</summary>
    BottomRight,
    /// <summary>Bottom-left corner of the anchor.</summary>
    BottomLeft,
}

namespace Flare.Abstractions;

/// <summary>
/// Where a component dialog is anchored within the viewport. <see cref="Center"/> is the classic
/// centered modal; <see cref="Bottom"/> presents the same component-dialog contract as a slide-up
/// bottom sheet (full-width, rounded top corners, optional grabber handle, safe-area padding).
/// </summary>
public enum DialogPosition
{
    /// <summary>Centered in the viewport (the default modal presentation).</summary>
    Center,
    /// <summary>Anchored to the bottom edge as a slide-up bottom sheet.</summary>
    Bottom,
}

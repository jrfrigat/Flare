namespace Flare.Components;

/// <summary>
/// Maximum width constraint applied to layout containers such as <c>FlareContainer</c> and the
/// content area of <c>FlareLayout</c> / <c>FlareLayoutContent</c>. Each value (except
/// <see cref="Full"/>) caps the width and centers the content horizontally.
/// </summary>
public enum ContainerMaxWidth
{
    /// <summary>Extra small: caps the content at 480px and centers it.</summary>
    Xs,

    /// <summary>Small: caps the content at 600px and centers it.</summary>
    Sm,

    /// <summary>Medium: caps the content at 960px and centers it.</summary>
    Md,

    /// <summary>Large: caps the content at 1280px and centers it.</summary>
    Lg,

    /// <summary>Extra large: caps the content at 1920px and centers it.</summary>
    Xl,

    /// <summary>No constraint: the content spans the full available width.</summary>
    Full,
}

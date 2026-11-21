namespace Flare.Components.IDE;

/// <summary>Body regions a dock group offers as drop targets. The tab row is a separate reorder surface.</summary>
[Flags]
public enum DockDropZones
{
    /// <summary>No body region; tabs can still be reordered and activated.</summary>
    None = 0,

    /// <summary>The center of the group: move the item into the group.</summary>
    Into = 1 << 0,

    /// <summary>The inline-start edge: split before the group.</summary>
    Start = 1 << 1,

    /// <summary>The inline-end edge: split after the group.</summary>
    End = 1 << 2,

    /// <summary>The block-start edge: split above the group.</summary>
    Top = 1 << 3,

    /// <summary>The block-end edge: split below the group.</summary>
    Bottom = 1 << 4,

    /// <summary>All five body regions.</summary>
    All = Into | Start | End | Top | Bottom
}

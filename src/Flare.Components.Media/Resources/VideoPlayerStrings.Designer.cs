using System.Globalization;
using System.Resources;

namespace Flare.Components.Resources;

/// <summary>
/// Localized accessible names for the media components, read through
/// <see cref="CultureInfo.CurrentUICulture"/>. The strings ship with the Media package rather than with
/// the core library, so a player keeps its own wording however the application names things.
/// </summary>
public static class VideoPlayerStrings
{
    private static readonly ResourceManager ResourceManager =
        new("Flare.Components.Resources.VideoPlayerStrings", typeof(VideoPlayerStrings).Assembly);

    private static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    /// <summary>Accessible name of the video element.</summary>
    public static string Label => Get(nameof(Label));

    /// <summary>Play button, shown while the video is paused.</summary>
    public static string Play => Get(nameof(Play));

    /// <summary>Pause button, shown while the video plays.</summary>
    public static string Pause => Get(nameof(Pause));

    /// <summary>Accessible name of the seek slider.</summary>
    public static string Seek => Get(nameof(Seek));

    /// <summary>Mute button, shown while the audio is audible.</summary>
    public static string Mute => Get(nameof(Mute));

    /// <summary>Unmute button, shown while the audio is muted.</summary>
    public static string Unmute => Get(nameof(Unmute));

    /// <summary>Accessible name of the volume slider.</summary>
    public static string Volume => Get(nameof(Volume));

    /// <summary>Accessible name of the playback speed selector.</summary>
    public static string PlaybackSpeed => Get(nameof(PlaybackSpeed));

    /// <summary>Fullscreen button, shown while the player is not fullscreen.</summary>
    public static string Fullscreen => Get(nameof(Fullscreen));

    /// <summary>Exit-fullscreen button, shown while the player is fullscreen.</summary>
    public static string ExitFullscreen => Get(nameof(ExitFullscreen));
}

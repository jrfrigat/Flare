namespace Flare.Components;

/// <summary>
/// Deprecated alias for <see cref="FlareZone"/>. Kept so existing <c>&lt;FlareSliderZone&gt;</c> markup
/// keeps compiling; new code should use <see cref="FlareZone"/>, which is the same component and also works
/// inside <see cref="FlareProgress"/> and <see cref="FlareMeter"/>.
/// </summary>
[Obsolete("Use FlareZone instead; it is the same zone and also works in FlareProgress and FlareMeter.")]
public sealed class FlareSliderZone : FlareZone
{
}

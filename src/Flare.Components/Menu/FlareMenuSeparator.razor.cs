namespace Flare.Components;

/// <summary>
/// A non-interactive rule between groups of entries in a <see cref="FlareMenu"/> or
/// <see cref="FlareSubMenu"/> panel. It reports <c>role="separator"</c> and takes no part in the
/// menu's keyboard model: it does not register as an entry, so the roving highlight skips it, and it
/// has no tab stop. Its look and spacing come from theme tokens.
/// </summary>
public partial class FlareMenuSeparator : FlareComponentBase
{
    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.Menu.Separator;
}

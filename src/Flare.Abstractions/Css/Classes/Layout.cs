namespace Flare.Css.Classes;

/// <summary>CSS classes for layout.</summary>
public static class Layout
{
    /// <summary>The <c>flare-layout</c> CSS class.</summary>
    public const string Root = "flare-layout";
    /// <summary>The <c>flare-layout--mobile</c> CSS class: the layout is below its mobile breakpoint,
    /// so push drawers become off-canvas overlays.</summary>
    public const string Mobile = "flare-layout--mobile";
    /// <summary>The <c>flare-layout--scrim-open</c> CSS class: a drawer is currently a floating
    /// overlay, so the scrim is shown over the content.</summary>
    public const string ScrimOpen = "flare-layout--scrim-open";
    /// <summary>The <c>flare-layout-appbar</c> CSS class.</summary>
    public const string AppBar = "flare-layout-appbar";
    /// <summary>The <c>flare-layout-appbar--dense</c> CSS class: a slimmer app bar.</summary>
    public const string AppBarDense = "flare-layout-appbar--dense";
    /// <summary>The <c>flare-layout-appbar__toggle</c> CSS class.</summary>
    public const string AppBarToggle = "flare-layout-appbar__toggle";
    /// <summary>The <c>flare-layout-appbar__toggle--open</c> CSS class.</summary>
    public const string AppbarToggleOpen = "flare-layout-appbar__toggle--open";
    /// <summary>The <c>flare-layout-content</c> CSS class: the main content region (the flexible track).</summary>
    public const string Content = "flare-layout-content";
    /// <summary>The <c>flare-layout__content-frame</c> CSS class.</summary>
    public const string ContentFrame = "flare-layout__content-frame";
    /// <summary>The <c>flare-layout__scrim</c> CSS class.</summary>
    public const string Scrim = "flare-layout__scrim";

    /// <summary>The <c>flare-layout-drawer</c> CSS class.</summary>
    public const string Drawer = "flare-layout-drawer";
    /// <summary>The <c>flare-layout-drawer__inner</c> CSS class.</summary>
    public const string DrawerInner = "flare-layout-drawer__inner";
    /// <summary>The <c>flare-layout-drawer--persistent</c> CSS class (push variant, hidden when closed).</summary>
    public const string DrawerPersistent = "flare-layout-drawer--persistent";
    /// <summary>The <c>flare-layout-drawer--mini</c> CSS class (collapses to an icon rail).</summary>
    public const string DrawerMini = "flare-layout-drawer--mini";
    /// <summary>The <c>flare-layout-drawer--temporary</c> CSS class (floats over content with a scrim).</summary>
    public const string DrawerTemporary = "flare-layout-drawer--temporary";
    /// <summary>The <c>flare-layout-drawer--responsive</c> CSS class (push on desktop, temporary on mobile).</summary>
    public const string DrawerResponsive = "flare-layout-drawer--responsive";
    /// <summary>The <c>flare-layout-drawer--permanent</c> CSS class (always visible, never collapses).</summary>
    public const string DrawerPermanent = "flare-layout-drawer--permanent";
    /// <summary>The <c>flare-layout-drawer--end</c> CSS class (anchored to the inline-end edge).</summary>
    public const string DrawerEnd = "flare-layout-drawer--end";
    /// <summary>The <c>flare-layout-drawer--open</c> CSS class: the drawer is open/expanded.</summary>
    public const string DrawerOpenMod = "flare-layout-drawer--open";
    /// <summary>The <c>flare-layout-drawer--floating</c> CSS class: the drawer is out of flow and floats
    /// over the content (overlay variant, or any push drawer on mobile).</summary>
    public const string DrawerFloating = "flare-layout-drawer--floating";
    /// <summary>The <c>flare-layout-drawer--hover-expand</c> CSS class: a collapsed Rail is temporarily
    /// expanded to its full width by hover or keyboard focus.</summary>
    public const string DrawerHoverExpand = "flare-layout-drawer--hover-expand";

    /// <summary>The <c>flare-layout-content--lg</c> CSS class.</summary>
    public const string ContentLg = "flare-layout-content--lg";
    /// <summary>The <c>flare-layout-content--md</c> CSS class.</summary>
    public const string ContentMd = "flare-layout-content--md";
    /// <summary>The <c>flare-layout-content--sm</c> CSS class.</summary>
    public const string ContentSm = "flare-layout-content--sm";
    /// <summary>The <c>flare-layout-content--xl</c> CSS class.</summary>
    public const string ContentXl = "flare-layout-content--xl";
    /// <summary>The <c>flare-layout-content--xs</c> CSS class.</summary>
    public const string ContentXs = "flare-layout-content--xs";
    /// <summary>The <c>flare-layout-content--align-start</c> CSS class.</summary>
    public const string ContentAlignStart = "flare-layout-content--align-start";
    /// <summary>The <c>flare-layout-content--align-end</c> CSS class.</summary>
    public const string ContentAlignEnd = "flare-layout-content--align-end";
    /// <summary>The <c>flare-layout-content--align-center</c> CSS class.</summary>
    public const string ContentAlignCenter = "flare-layout-content--align-center";
}

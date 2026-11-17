namespace Flare.Css.Classes.Ide;

/// <summary><c>FlareIdeLayout</c> shell (rails, panels, resizers, regions).</summary>
public static class Layout
{
    public const string Root = "flare-ide-layout";
    public const string Body = "flare-ide-layout__body";
    /// <summary>Body modifier that removes the left docking column.</summary>
    public const string BodyHideLeft = "flare-ide-layout__body--hide-left";
    /// <summary>Body modifier that removes the right docking column.</summary>
    public const string BodyHideRight = "flare-ide-layout__body--hide-right";
    public const string Bottom = "flare-ide-layout__bottom";
    /// <summary>Tool-window header drawn above the bottom panel content.</summary>
    public const string BottomHeader = "flare-ide-layout__bottom-header";
    /// <summary>Host labels or actions at the start of the bottom header.</summary>
    public const string BottomHeaderContent = "flare-ide-layout__bottom-header-content";
    /// <summary>Toolbar that holds the typed bottom panel controls.</summary>
    public const string BottomHeaderActions = "flare-ide-layout__bottom-header-actions";
    /// <summary>Bottom panel body that keeps its header fixed and scrolls only the content.</summary>
    public const string BottomBody = "flare-ide-layout__bottom-body";
    /// <summary>Scrolling content area below the bottom panel header.</summary>
    public const string BottomContent = "flare-ide-layout__bottom-content";
    /// <summary>Pin icon in the bottom header; the unpinned modifier rotates it.</summary>
    public const string BottomPinIcon = "flare-ide-layout__pin-icon";
    /// <summary>Pin icon state for an unpinned bottom panel.</summary>
    public const string BottomPinUnpinned = "flare-ide-layout__pin-icon--unpinned";
    /// <summary>Slot in the Center dock row that hosts a custom main tab strip.</summary>
    public const string CenterHeader = "flare-ide-layout__center-header";
    public const string Content = "flare-ide-layout__content";
    /// <summary>Document panel that remains mounted across moves.</summary>
    public const string Document = "flare-ide-layout__document";
    /// <summary>Document controls in a dock region.</summary>
    public const string DockTabs = "flare-ide-layout__dock-tabs";
    /// <summary>Container for one dock tab and its actions.</summary>
    public const string DockTab = "flare-ide-layout__dock-tab";
    /// <summary>Button that selects the document represented by a dock tab.</summary>
    public const string DockSelect = "flare-ide-layout__dock-select";
    /// <summary>Unsaved-changes marker in a dock tab.</summary>
    public const string DockModified = "flare-ide-layout__dock-modified";
    /// <summary>Close button in a dock tab.</summary>
    public const string DockClose = "flare-ide-layout__dock-close";
    public const string Main = "flare-ide-layout__main";
    public const string MenuBar = "flare-ide-layout__menubar";
    public const string Panel = "flare-ide-layout__panel";
    public const string PanelCollapsed = "flare-ide-layout__panel--collapsed";
    public const string PanelFloating = "flare-ide-layout__panel--floating";
    public const string PanelLeft = "flare-ide-layout__panel--left";
    public const string PanelRight = "flare-ide-layout__panel--right";
    /// <summary>Side region whose rail lists one entry per hosted tool.</summary>
    public const string PanelToolRail = "flare-ide-layout__panel--tool-rail";
    public const string PanelBody = "flare-ide-layout__panel-body";
    public const string Rail = "flare-ide-layout__rail";
    /// <summary>Rail holding one entry per tool in a per-tool auto-hide region.</summary>
    public const string ToolRail = "flare-ide-layout__tool-rail";
    /// <summary>One tool entry on a per-tool rail.</summary>
    public const string ToolRailEntry = "flare-ide-layout__tool-rail-entry";
    /// <summary>Tool entry whose flyout is open.</summary>
    public const string ToolRailEntryActive = "flare-ide-layout__tool-rail-entry--active";
    /// <summary>Rail highlighted while a document is dragged over it.</summary>
    public const string RailDropTarget = "flare-ide-layout__rail--drop-target";
    public const string RailTitle = "flare-ide-layout__rail-title";
    public const string Resizer = "flare-ide-layout__resizer";
    public const string Ribbon = "flare-ide-layout__ribbon";
    public const string StatusBar = "flare-ide-layout__statusbar";
}

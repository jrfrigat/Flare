using System.Globalization;
using System.Resources;

namespace Flare.Components.IDE;

/// <summary>
/// Localized accessible names and default labels of the IDE components, read through
/// <see cref="CultureInfo.CurrentUICulture"/>. The strings ship with the IDE package rather than with
/// the core library, so an IDE keeps its own wording however the application names things.
/// </summary>
public static class IdeStrings
{
    private static readonly ResourceManager ResourceManager =
        new("Flare.Components.IDE.IdeStrings", typeof(IdeStrings).Assembly);

    private static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    /// <summary>Accessible label of the backstage's back/close button.</summary>
    public static string BackstageClose => Get("Backstage_Close");

    /// <summary>Accessible label of the rail that expands a collapsed layout panel.</summary>
    public static string LayoutExpandPanel => Get("Layout_ExpandPanel");

    /// <summary>Accessible name of the document tab strip.</summary>
    public static string DocTabsLabel => Get("DocTabs_Label");

    /// <summary>Accessible label of the modified-document marker.</summary>
    public static string DocTabsModified => Get("DocTabs_Modified");

    /// <summary>Accessible label of a document tab's close button.</summary>
    public static string DocTabsClose => Get("DocTabs_Close");

    /// <summary>Accessible label of the button listing tabs that overflow the strip.</summary>
    public static string DocTabsHidden => Get("DocTabs_Hidden");

    /// <summary>Accessible label of the formula bar's name box.</summary>
    public static string FormulaBarNameBox => Get("FormulaBar_NameBox");

    /// <summary>Accessible label of the formula bar's editor.</summary>
    public static string FormulaBarFormula => Get("FormulaBar_Formula");

    /// <summary>Accessible label of the sheet tabs' add button.</summary>
    public static string SheetTabsAdd => Get("SheetTabs_Add");

    /// <summary>Accessible label of the control that expands a collapsed tool panel.</summary>
    public static string ToolPanelExpand => Get("ToolPanel_Expand");

    /// <summary>Accessible label of the control that collapses an expanded tool panel.</summary>
    public static string ToolPanelCollapse => Get("ToolPanel_Collapse");
}

using System.Globalization;
using System.Resources;

namespace Flare.Components.IDE;

/// <summary>
/// Localized accessible names and default labels of the IDE components, read through
/// <see cref="CultureInfo.CurrentUICulture"/>. The strings ship with the IDE package rather than with
/// the core library, so an IDE keeps its own wording however the application names things.
/// </summary>
public sealed class IdeStrings
{
    private IdeStrings() { }
    private static readonly ResourceManager ResourceManager =
        new("Flare.Components.IDE.IdeStrings", typeof(IdeStrings).Assembly);

    private static string Get(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    /// <summary>Accessible label of the backstage's back/close button.</summary>
    public static string BackstageClose => Get("Backstage_Close");

    /// <summary>Accessible label of the formula bar's name box.</summary>
    public static string FormulaBarNameBox => Get("FormulaBar_NameBox");

    /// <summary>Accessible label of the formula bar's editor.</summary>
    public static string FormulaBarFormula => Get("FormulaBar_Formula");

    /// <summary>Accessible label of the control that expands a collapsed tool panel.</summary>
    public static string ToolPanelExpand => Get("ToolPanel_Expand");

    /// <summary>Accessible label of the control that collapses an expanded tool panel.</summary>
    public static string ToolPanelCollapse => Get("ToolPanel_Collapse");

    /// <summary>Accessible name of the editor breadcrumb row's action area.</summary>
    public static string EditorBarActions => Get("EditorBar_Actions");
}

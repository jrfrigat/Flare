using Flare.Components;

namespace Flare.Gallery.Models;

/// <summary>
/// The category a component demo page belongs to in the navigation menu. The enum order is the
/// display order of the groups in the sidebar.
/// </summary>
public enum ComponentGroup
{
    /// <summary>Text-entry fields (value typed by the user).</summary>
    Field,
    /// <summary>Pointer-driven inputs (value set by mouse/touch).</summary>
    Input,
    /// <summary>File pickers and drop zones.</summary>
    Files,
    /// <summary>Form composition and validation.</summary>
    Forms,
    /// <summary>Buttons and action triggers.</summary>
    Buttons,
    /// <summary>Layout primitives and surfaces.</summary>
    Layout,
    /// <summary>Navigation components.</summary>
    Navigation,
    /// <summary>Read-only data display.</summary>
    DataDisplay,
    /// <summary>Tabular data grids.</summary>
    DataGrid,
    /// <summary>Date and time pickers.</summary>
    DateTime,
    /// <summary>Feedback, status and overlay surfaces.</summary>
    Feedback,
    /// <summary>Media (images, video, carousels, QR).</summary>
    Media,
    /// <summary>Design foundations (icons, typography).</summary>
    Foundations,
    /// <summary>Utility / helper components.</summary>
    Utilities,
    /// <summary>IDE-style layout shell.</summary>
    Ide,
}

/// <summary>One component demo page in the gallery navigation/search catalog.</summary>
/// <param name="Href">Route of the demo page.</param>
/// <param name="Name">English display name (matches the sidebar label).</param>
/// <param name="TitleKey">
/// <c>GalleryStrings</c> resource key of the page title, used to pull the localized (EN+RU) name for
/// search. Null when the page has no dedicated title resource (search falls back to <see cref="Name"/>).
/// </param>
/// <param name="Group">The sidebar group this page belongs to.</param>
public sealed record ComponentEntry(string Href, string Name, string? TitleKey, ComponentGroup Group);

/// <summary>Display metadata (label + icon) for a <see cref="ComponentGroup"/>.</summary>
public sealed record ComponentGroupMeta(ComponentGroup Group, string Label, FlareIcon Icon);

/// <summary>
/// Single source of truth for the component demo pages: the sidebar groups and the global search
/// index are both built from this list, so routes are never duplicated across the two.
/// Component names are English-only by request (no localization of component names themselves).
/// </summary>
public static class ComponentCatalog
{
    /// <summary>Groups in sidebar display order, with their label and (typed) icon.</summary>
    public static readonly IReadOnlyList<ComponentGroupMeta> Groups = new[]
    {
        new ComponentGroupMeta(ComponentGroup.Field,       "Field",              MaterialDesign3Icons.Regular.TextFields),
        new ComponentGroupMeta(ComponentGroup.Input,       "Input",              MaterialDesign3Icons.Regular.AdsClick),
        new ComponentGroupMeta(ComponentGroup.Files,       "Files",              FlareIcons.UploadFile),
        new ComponentGroupMeta(ComponentGroup.Forms,       "Forms",              MaterialDesign3Icons.Regular.DynamicForm),
        new ComponentGroupMeta(ComponentGroup.Buttons,     "Buttons & Actions",  MaterialDesign3Icons.Regular.Widgets),
        new ComponentGroupMeta(ComponentGroup.Layout,      "Layout & Surfaces",  MaterialDesign3Icons.Regular.Dashboard),
        new ComponentGroupMeta(ComponentGroup.Navigation,  "Navigation",         FlareIcons.Menu),
        new ComponentGroupMeta(ComponentGroup.DataDisplay, "Data Display",       MaterialDesign3Icons.Regular.ViewList),
        new ComponentGroupMeta(ComponentGroup.DataGrid,    "Data Grid",          MaterialDesign3Icons.Regular.TableChart),
        new ComponentGroupMeta(ComponentGroup.DateTime,    "Date & Time",        FlareIcons.CalendarMonth),
        new ComponentGroupMeta(ComponentGroup.Feedback,    "Feedback & Overlays",FlareIcons.Notifications),
        new ComponentGroupMeta(ComponentGroup.Media,       "Media",              MaterialDesign3Icons.Regular.PermMedia),
        new ComponentGroupMeta(ComponentGroup.Foundations, "Foundations",        MaterialDesign3Icons.Regular.Foundation),
        new ComponentGroupMeta(ComponentGroup.Utilities,   "Utilities",          MaterialDesign3Icons.Regular.Handyman),
        new ComponentGroupMeta(ComponentGroup.Ide,         "IDE",                FlareIcons.Code),
    };

    /// <summary>Every component demo page, grouped via <see cref="ComponentEntry.Group"/>.</summary>
    public static readonly IReadOnlyList<ComponentEntry> All = new[]
    {
        // -- Field (typed value) --------------------------------------------------
        new ComponentEntry("/components/autocomplete",      "Autocomplete",            "Autocomplete_Title",    ComponentGroup.Field),
        new ComponentEntry("/components/masked-field",      "Masked Field",            "InputMask_Title",       ComponentGroup.Field),
        new ComponentEntry("/components/numeric-field",     "Numeric Field",           "NumericField_Title",    ComponentGroup.Field),
        new ComponentEntry("/components/otp-field",         "OTP Field",               "OtpField_Title",        ComponentGroup.Field),
        new ComponentEntry("/components/password-field",    "Password Field",          "PasswordInput_Title",   ComponentGroup.Field),
        new ComponentEntry("/components/rich-text-editor",  "Rich Text Editor",        "RichTextEditor_Title",  ComponentGroup.Field),
        new ComponentEntry("/components/tag-field",         "Tag Field",               "TagInput_Title",        ComponentGroup.Field),
        new ComponentEntry("/components/textarea",          "Text Area",               "TextArea_Title",        ComponentGroup.Field),
        new ComponentEntry("/components/text-field",        "Text Field",              "Input_Title",           ComponentGroup.Field),

        // -- Input (pointer-driven) ----------------------------------------------
        new ComponentEntry("/components/checkbox",          "Checkbox",                "Checkbox_Title",        ComponentGroup.Input),
        new ComponentEntry("/components/colorpicker",       "Color Picker",            "ColorPicker_Title",     ComponentGroup.Input),
        new ComponentEntry("/components/multi-select",      "Multi Select",            "MultiSelect_Title",     ComponentGroup.Input),
        new ComponentEntry("/components/radio",             "Radio Group",             "Radio_Title",           ComponentGroup.Input),
        new ComponentEntry("/components/rating",            "Rating",                  "Rating_Title",          ComponentGroup.Input),
        new ComponentEntry("/components/select",            "Select",                  "Select_Title",          ComponentGroup.Input),
        new ComponentEntry("/components/signaturepad",      "Signature Pad",           "SignaturePad_Title",    ComponentGroup.Input),
        new ComponentEntry("/components/slider",            "Slider",                  "Slider_Title",          ComponentGroup.Input),
        new ComponentEntry("/components/switch",            "Switch",                  "Switch_Title",          ComponentGroup.Input),
        new ComponentEntry("/components/toggle-button",     "Toggle Button",           "ToggleButton_Title",    ComponentGroup.Input),
        new ComponentEntry("/components/transfer",          "Transfer / Dual List",    "Transfer_Title",        ComponentGroup.Input),

        // -- Files ----------------------------------------------------------------
        new ComponentEntry("/components/fileupload",        "File Upload",             "FileUpload_Title",      ComponentGroup.Files),

        // -- Forms ----------------------------------------------------------------
        new ComponentEntry("/components/form-builder",      "Form Builder",            "FormBuilder_Title",     ComponentGroup.Forms),
        new ComponentEntry("/components/forms",             "Form Validation",         "Forms_Title",           ComponentGroup.Forms),

        // -- Buttons & Actions ----------------------------------------------------
        new ComponentEntry("/components/buttons",           "Button",                  "Buttons_Title",         ComponentGroup.Buttons),
        new ComponentEntry("/components/button-group",      "Button Group",            "ButtonGroup_Title",     ComponentGroup.Buttons),
        new ComponentEntry("/components/fab-menu",          "FAB Menu",                "FabMenu_Title",         ComponentGroup.Buttons),
        new ComponentEntry("/components/fab",               "Floating Action Button",  "Fab_Title",             ComponentGroup.Buttons),
        new ComponentEntry("/components/icon-button",       "Icon Button",             "IconButton_Title",      ComponentGroup.Buttons),
        new ComponentEntry("/components/split-button",      "Split Button",            "SplitButton_Title",     ComponentGroup.Buttons),

        // -- Layout & Surfaces ----------------------------------------------------
        new ComponentEntry("/components/accordion",         "Accordion",               "Accordion_Title",       ComponentGroup.Layout),
        new ComponentEntry("/components/cards",             "Card",                    "Cards_Title",           ComponentGroup.Layout),
        new ComponentEntry("/components/collapse",          "Collapse",                "Collapse_Title",        ComponentGroup.Layout),
        new ComponentEntry("/components/container",         "Container",               "Container_Title",       ComponentGroup.Layout),
        new ComponentEntry("/components/divider",           "Divider",                 "Divider_Title",         ComponentGroup.Layout),
        new ComponentEntry("/components/grid",              "Grid",                    "Grid_Title",            ComponentGroup.Layout),
        new ComponentEntry("/components/layout-shell",      "Layout Shell",            "LayoutShell_Title",     ComponentGroup.Layout),
        new ComponentEntry("/components/paper",             "Paper",                   "Paper_Title",           ComponentGroup.Layout),
        new ComponentEntry("/components/resizable",         "Resizable",               "Resizable_Title",       ComponentGroup.Layout),
        new ComponentEntry("/components/responsive",        "Responsive & Adaptivity", "Responsive_Title",      ComponentGroup.Layout),
        new ComponentEntry("/components/splitter",          "Splitter",                "Splitter_Title",        ComponentGroup.Layout),
        new ComponentEntry("/components/stack",             "Stack",                   "Stack_Title",           ComponentGroup.Layout),

        // -- Navigation -----------------------------------------------------------
        new ComponentEntry("/components/bottomnav",         "Bottom Nav",              "BottomNav_Title",       ComponentGroup.Navigation),
        new ComponentEntry("/components/breadcrumb",        "Breadcrumb",              "Breadcrumb_Title",      ComponentGroup.Navigation),
        new ComponentEntry("/components/drawer",            "Drawer",                  "Drawer_Title",          ComponentGroup.Navigation),
        new ComponentEntry("/components/link",              "Link",                    "Link_Title",            ComponentGroup.Navigation),
        new ComponentEntry("/components/menu",              "Menu",                    "Menu_Title",            ComponentGroup.Navigation),
        new ComponentEntry("/components/navmenu",           "Nav Menu",                "NavMenu_Title",         ComponentGroup.Navigation),
        new ComponentEntry("/components/on-this-page",      "On This Page",            "OnThisPage_Title",      ComponentGroup.Navigation),
        new ComponentEntry("/components/pagination",        "Pagination",              "Pagination_Title",      ComponentGroup.Navigation),
        new ComponentEntry("/components/scroll-top",        "Scroll Top",              "ScrollTop_Title",       ComponentGroup.Navigation),
        new ComponentEntry("/components/stepper",           "Stepper",                 "Stepper_Title",         ComponentGroup.Navigation),
        new ComponentEntry("/components/tabs",              "Tabs",                    "Tabs_Title",            ComponentGroup.Navigation),

        // -- Data Display ---------------------------------------------------------
        new ComponentEntry("/components/avatar",            "Avatar",                  "Avatar_Title",          ComponentGroup.DataDisplay),
        new ComponentEntry("/components/avatar-group",      "Avatar Group",            "AvatarGroup_Title",     ComponentGroup.DataDisplay),
        new ComponentEntry("/components/badge",             "Badge",                   "Badge_Title",           ComponentGroup.DataDisplay),
        new ComponentEntry("/components/charts",            "Charts",                  "Charts_Title",          ComponentGroup.DataDisplay),
        new ComponentEntry("/components/chip",              "Chip",                    "Chip_Title",            ComponentGroup.DataDisplay),
        new ComponentEntry("/components/chip-group",        "Chip Group",              "ChipGroup_Title",       ComponentGroup.DataDisplay),
        new ComponentEntry("/components/data-tree",         "Data Tree",               "DataTree_Title",        ComponentGroup.DataDisplay),
        new ComponentEntry("/components/description-list",  "Description List",        "DescriptionList_Title", ComponentGroup.DataDisplay),
        new ComponentEntry("/components/kanban",            "Kanban Board",            "Kanban_Title",          ComponentGroup.DataDisplay),
        new ComponentEntry("/components/list",              "List",                    "List_Title",            ComponentGroup.DataDisplay),
        new ComponentEntry("/components/markdown",          "Markdown",                "Markdown_Title",        ComponentGroup.DataDisplay),
        new ComponentEntry("/components/timeline",          "Timeline",                "Timeline_Title",        ComponentGroup.DataDisplay),
        new ComponentEntry("/components/tree",              "Tree View",               "Tree_Title",            ComponentGroup.DataDisplay),
        new ComponentEntry("/components/virtual-list",      "Virtual List",            "VirtualList_Title",     ComponentGroup.DataDisplay),

        // -- Data Grid ------------------------------------------------------------
        new ComponentEntry("/components/datagrid",          "Data Grid",               "DataGrid_Title",        ComponentGroup.DataGrid),
        new ComponentEntry("/components/table",             "Table",                   "Table_Title",           ComponentGroup.DataGrid),

        // -- Date & Time ----------------------------------------------------------
        new ComponentEntry("/components/calendar",          "Calendar",                "Calendar_Title",        ComponentGroup.DateTime),
        new ComponentEntry("/components/date-picker",       "Date Picker",             "DatePicker_Title",      ComponentGroup.DateTime),
        new ComponentEntry("/components/date-range-picker", "Date Range Picker",       "DateRangePicker_Title", ComponentGroup.DateTime),
        new ComponentEntry("/components/date-time-picker",  "Date Time Picker",        "DateTimePicker_Title",  ComponentGroup.DateTime),
        new ComponentEntry("/components/time-picker",       "Time Picker",             "TimePicker_Title",      ComponentGroup.DateTime),

        // -- Feedback & Overlays --------------------------------------------------
        new ComponentEntry("/components/alerts",            "Alert",                   "Alerts_Title",          ComponentGroup.Feedback),
        new ComponentEntry("/components/confirm",           "Confirm Dialog",          "Confirm_Title",         ComponentGroup.Feedback),
        new ComponentEntry("/components/dialog",            "Dialog",                  "Dialog_Title",          ComponentGroup.Feedback),
        new ComponentEntry("/components/empty-state",       "Empty State",             "EmptyState_Title",      ComponentGroup.Feedback),
        new ComponentEntry("/components/overlay",           "Overlay",                 "Overlay_Title",         ComponentGroup.Feedback),
        new ComponentEntry("/components/popover",           "Popover",                 "Popover_Title",         ComponentGroup.Feedback),
        new ComponentEntry("/components/meter",             "Meter",                   "Meter_Title",           ComponentGroup.Feedback),
        new ComponentEntry("/components/progress",          "Progress",                "Progress_Title",        ComponentGroup.Feedback),
        new ComponentEntry("/components/skeleton",          "Skeleton",                "Skeleton_Title",        ComponentGroup.Feedback),
        new ComponentEntry("/components/snackbar",          "Snackbar",                "Snackbar_Title",        ComponentGroup.Feedback),
        new ComponentEntry("/components/tooltip",           "Tooltip",                 "Tooltip_Title",         ComponentGroup.Feedback),

        // -- Media ----------------------------------------------------------------
        new ComponentEntry("/components/carousel",          "Carousel",                "Carousel_Title",        ComponentGroup.Media),
        new ComponentEntry("/components/image",             "Image",                   "Image_Title",           ComponentGroup.Media),
        new ComponentEntry("/components/qrcode",            "QR Code",                 "QrCode_Title",          ComponentGroup.Media),
        new ComponentEntry("/components/videoplayer",       "Video Player",            "VideoPlayer_Title",     ComponentGroup.Media),

        // -- Foundations ----------------------------------------------------------
        new ComponentEntry("/components/icon",              "Icon",                    "Icon_Title",            ComponentGroup.Foundations),
        new ComponentEntry("/components/icons",             "Icons",                   "Icons_Title",           ComponentGroup.Foundations),
        new ComponentEntry("/components/typography",        "Typography",              "Typography_Title",      ComponentGroup.Foundations),

        // -- Utilities ------------------------------------------------------------
        new ComponentEntry("/components/clipboard",         "Clipboard",               "Clipboard_Title",       ComponentGroup.Utilities),
        new ComponentEntry("/components/color-mode-toggle", "Color Mode Toggle",       "ColorModeToggle_Title", ComponentGroup.Utilities),
        new ComponentEntry("/components/highlighter",       "Highlighter",             "Highlighter_Title",     ComponentGroup.Utilities),
        new ComponentEntry("/components/infinite-scroll",   "Infinite Scroll",         "InfiniteScroll_Title",  ComponentGroup.Utilities),
        new ComponentEntry("/components/lazy",              "Lazy Render",             "Lazy_Title",            ComponentGroup.Utilities),
        new ComponentEntry("/components/shortcuts",         "Shortcuts",               "Shortcuts_Title",       ComponentGroup.Utilities),

        // -- IDE ------------------------------------------------------------------
        new ComponentEntry("/components/backstage",            "Backstage",            "Ide_Backstage_Title",    ComponentGroup.Ide),
        new ComponentEntry("/components/document-tabs",        "Document Tabs",        "Ide_DocTabs_Title",      ComponentGroup.Ide),
        new ComponentEntry("/components/formula-bar",          "Formula Bar",          "Ide_FormulaBar_Title",   ComponentGroup.Ide),
        new ComponentEntry("/components/ide",                  "IDE Layout",           "Ide_Title",              ComponentGroup.Ide),
        new ComponentEntry("/components/menu-bar",             "Menu Bar",             "Ide_MenuBar_Title",      ComponentGroup.Ide),
        new ComponentEntry("/components/property-grid",        "Property Grid",        "Ide_PropertyGrid_Title", ComponentGroup.Ide),
        new ComponentEntry("/components/quick-access-toolbar", "Quick Access Toolbar", "Ide_Qat_Title",          ComponentGroup.Ide),
        new ComponentEntry("/components/ribbon",               "Ribbon",               "Ide_Ribbon_Title",       ComponentGroup.Ide),
        new ComponentEntry("/components/sheet-tabs",           "Sheet Tabs",           "Ide_SheetTabs_Title",    ComponentGroup.Ide),
        new ComponentEntry("/components/status-bar",           "Status Bar",           "Ide_StatusBar_Title",    ComponentGroup.Ide),
        new ComponentEntry("/components/tool-panel",           "Tool Panel",           "Ide_ToolPanel_Title",    ComponentGroup.Ide),
        new ComponentEntry("/components/toolbar",              "Toolbar",              "Ide_Toolbar_Title",      ComponentGroup.Ide),
    };

    /// <summary>Components in a group, sorted by English name.</summary>
    public static IEnumerable<ComponentEntry> InGroup(ComponentGroup group) =>
        All.Where(e => e.Group == group).OrderBy(e => e.Name, StringComparer.Ordinal);
}

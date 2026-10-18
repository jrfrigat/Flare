namespace Flare.Components;

/// <summary>An entry the menu's roving highlight can land on: an actionable item or a submenu trigger.</summary>
internal interface IMenuEntry
{
    /// <summary>Whether the entry is skipped by keyboard navigation.</summary>
    bool Disabled { get; }
    /// <summary>The id referenced by the menu's <c>aria-activedescendant</c>.</summary>
    string ItemId { get; }
    /// <summary>Runs the entry's own action, or opens the submenu for a submenu trigger.</summary>
    Task ActivateAsync();
    /// <summary>Whether the entry opens a submenu.</summary>
    bool HasSubmenu { get; }
    /// <summary>Whether the entry's submenu is currently open.</summary>
    bool IsSubmenuOpen { get; }
    /// <summary>Opens the submenu and moves focus into it.</summary>
    Task OpenSubmenuAsync();
    /// <summary>Closes the submenu and returns focus to its trigger.</summary>
    Task CloseSubmenuAsync();
}

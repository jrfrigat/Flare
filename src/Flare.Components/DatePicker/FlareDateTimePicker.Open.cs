using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// Opening the panel from a click in the field rather than from the toggle (TASK-190).
public partial class FlareDateTimePicker
{
    // A click in the field opens the panel as well; focus stays in the field (PickerPopup.FromField), so the user
    // can go on typing. Arrow Down moves into the calendar; Escape, or Tab out of the field, closes the panel.
    private Task OpenFromField()
    {
        if (_open || !AllowPicker || Disabled || ReadOnly) return Task.CompletedTask;
        _popup.OpenFromField();
        return TogglePanel();
    }

    // Wired only while a field-opened panel is showing, so typing into a closed field raises no key events.
    private EventCallback<KeyboardEventArgs> FieldKeyDown => _open && _popupState?.FromField == true
        ? EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleFieldKeyDown)
        : default;

    private Task HandleFieldKeyDown(KeyboardEventArgs e) => e.Key switch
    {
        "ArrowDown" => _popup.EnterAsync(_panelEl, () => _grid?.FocusCursorAsync() ?? Task.CompletedTask),
        "Escape" or "Tab" => CloseAsync(),
        _ => Task.CompletedTask,
    };
}

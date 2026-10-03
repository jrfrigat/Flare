using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Flare.Components;

// The Date / Time switch of the tabbed popup as an APG tab list: arrows and Home/End pick a tab and move focus to it,
// and only the picked tab is in the Tab order (TASK-166).
public partial class FlareDateTimePicker
{
    private readonly ElementReference[] _tabRefs = new ElementReference[2];

    private string TabId(int tab) => $"{ControlId}-tab-{(tab == 0 ? "date" : "time")}";
    private string TabPanelId => $"{ControlId}-tabpanel";

    private async Task HandleTabKeyDown(KeyboardEventArgs e)
    {
        int? target = e.Key switch
        {
            "ArrowRight" or "ArrowLeft" => 1 - _activeTab,
            "Home" => 0,
            "End" => 1,
            _ => null,
        };
        if (target is not { } tab) return;
        _activeTab = tab;
        try { await _tabRefs[tab].FocusAsync(); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }
}

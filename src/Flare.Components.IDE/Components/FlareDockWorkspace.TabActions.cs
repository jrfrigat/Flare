using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.IDE;

public partial class FlareDockWorkspace
{
    private FlareMenu? _tabContextMenu;
    private string? _contextTab;

    private EventCallback<MouseEventArgs> TabContextMenuHandler(string key) =>
        UseTabContextMenu
            ? EventCallback.Factory.Create<MouseEventArgs>(this, args => OpenTabContextMenuAsync(key, args))
            : default;

    private async Task OpenTabContextMenuAsync(string key, MouseEventArgs args)
    {
        if (_tabContextMenu is not { } menu || !_index.Owners.TryGetValue(key, out var group) ||
            (!CanCloseTab(group, key) && !TabActions(group, key).Any()))
        {
            return;
        }

        if (_contextTab is not null)
        {
            await menu.CloseAsync();
        }

        _contextTab = key;
        await menu.OpenAtAsync(args.ClientX, args.ClientY);
    }

    private void TabContextMenuToggled(bool open)
    {
        if (!open)
        {
            _contextTab = null;
        }
    }

    private bool CanCloseTab(DockGroup group, string key) =>
        _catalog[key].CanClose && MayRequest(DockAction.Close, key, group.Key);

    private string CloseTabLabel(string key) => string.Format(Text("Dock_CloseTab"), _catalog[key].Title);

    private IEnumerable<(string Label, Func<Task> Invoke)> TabActions(DockGroup group, string key)
    {
        foreach (var target in _index.Groups.Values)
        {
            if (MayRequest(DockAction.Move, key, target.Key))
            {
                if (target.Key == group.Key)
                {
                    var index = group.Items.IndexOf(key);
                    if (index > 0)
                        yield return (Text("Dock_Previous"), () => MoveAsync(key, target, index - 1));
                    if (index < group.Items.Length - 1)
                        yield return (Text("Dock_Next"), () => MoveAsync(key, target, index + 1));
                }
                else
                {
                    if (HasZone(DockDropZones.Into))
                        yield return (string.Format(Text("Dock_MoveTo"), TabListLabel(target)),
                            () => MoveAsync(key, target, target.Items.Length));

                    for (var index = 0; index < target.Items.Length; index++)
                    {
                        var position = index;
                        var title = _catalog[target.Items[index]].Title;
                        yield return (string.Format(Text("Dock_MoveBefore"), title), () => MoveAsync(key, target, position));
                        yield return (string.Format(Text("Dock_MoveAfter"), title), () => MoveAsync(key, target, position + 1));
                    }
                }
            }

            if (target.Items.Length <= (target.Key == group.Key ? 1 : 0) || !MayRequest(DockAction.Split, key, target.Key))
                continue;

            var label = TabListLabel(target);
            if (HasZone(DockDropZones.Start))
                yield return (string.Format(Text("Dock_SplitBefore"), label), () => SplitAsync(key, target, DockAxis.Horizontal, true));
            if (HasZone(DockDropZones.End))
                yield return (string.Format(Text("Dock_SplitAcross"), label), () => SplitAsync(key, target, DockAxis.Horizontal, false));
            if (HasZone(DockDropZones.Top))
                yield return (string.Format(Text("Dock_SplitAbove"), label), () => SplitAsync(key, target, DockAxis.Vertical, true));
            if (HasZone(DockDropZones.Bottom))
                yield return (string.Format(Text("Dock_SplitBelow"), label), () => SplitAsync(key, target, DockAxis.Vertical, false));
        }
    }
}

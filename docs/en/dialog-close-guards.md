# Protecting unsaved changes in dialogs

This guide describes the dialog changes in 0.52.0.

Place `FlareDialogCloseGuard` inside a dialog's `FlareForm` or `EditForm`. It reads the surrounding
`EditContext.IsModified()` and asks before discarding edits. Unchanged forms close immediately.
The default confirmation uses localized Flare strings and requires a `FlareDialogProvider` in your
layout, including when the protected dialog is inline.

Bind Flare fields with both `@bind-Value` and `For="@(() => model.Property)"`: `For` connects changes
to the form's `EditContext`. A value binding alone does not mark the form modified.

## A form with save and cancel

This reusable service dialog edits a working copy. Save marks the form unmodified only after the
save succeeds, then closes with the result. Cancel leaves the caller's original data untouched.

```razor
@using Microsoft.AspNetCore.Components.Forms

<FlareForm Model="_model" OnValidSubmit="Save">
    <FlareDialogCloseGuard />
    <FlareTextField @bind-Value="_model.Name" For="@(() => _model.Name)" Label="Name" />
    <FlareButton OnClick="Cancel">Cancel</FlareButton>
    <FlareButton Type="ButtonType.Submit">Save</FlareButton>
</FlareForm>

@code {
    [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
    [Parameter] public string Name { get; set; } = "";
    private readonly Profile _model = new();

    protected override void OnInitialized() => _model.Name = Name;

    private async Task Save(EditContext context)
    {
        // Await persistence here, if needed. Keep the form dirty if saving fails.
        context.MarkAsUnmodified();
        await Dialog.CloseAsync(_model.Name);
    }

    private Task Cancel() => Dialog.CancelAsync();

    private sealed class Profile
    {
        public string Name { get; set; } = "";
    }
}
```

Open it with `DialogService.ShowAsync<ProfileDialog>("Edit profile", parameters)` and read the saved
name with `result.GetData<string>()` when `result.Cancelled` is false. A sheet uses the same content
and protection. The Gallery dialog page demonstrates both service dialogs and an inline form.

For an inline dialog, keep `@bind-Visible` and add an `@ref`:

```razor
<FlareDialog @ref="_dialog" @bind-Visible="_visible" Title="Edit profile">
    @* Form and guard go here. Cancel calls _dialog.CloseAsync(). *@
</FlareDialog>

@code {
    private FlareDialog _dialog = default!;
    private bool _visible;
}
```

## Explicit state and custom confirmation

Outside a form, supply `HasUnsavedChanges`. Its default `null` reads the form context; explicit
`false` disables this guard even if the form is modified.

```razor
<FlareDialogCloseGuard HasUnsavedChanges="_dirty"
                       Title="Discard this draft?"
                       Message="Your draft has not been saved."
                       ConfirmLabel="Discard" CancelLabel="Keep editing" />
```

After saving, update the state and let that parameter reach the guard before requesting closure.
With `EditContext`, `MarkAsUnmodified()` takes effect immediately, including within the save handler.

`ConfirmClose` replaces the default prompt. It receives the close reason and returns `true` to allow
closure or `false` to keep the dialog open. The handler is called only when the guard detects edits:

```razor
<FlareDialogCloseGuard HasUnsavedChanges="_dirty" ConfirmClose="ConfirmDiscard" />

@code {
    private async ValueTask<bool> ConfirmDiscard(DialogCloseReason reason) =>
        await DialogService.ConfirmAsync("Discard draft?", "Continue without saving?") == true;
}
```

Custom content can also receive `[CascadingParameter] DialogCloseContext` and register its own
`Func<DialogCloseReason, ValueTask<bool>>` with `RegisterGuard`. Keep the returned `IDisposable` and
dispose it when the content is removed. The dialog instance exposes the same context through
`FlareDialogInstance.CloseContext`. A context accepts one close attempt at a time, so repeated
clicks cannot produce several prompts. `BeforeClose` runs first, then each registered guard;
any `false` vetoes the attempt.

## Close routes and migration

`DialogCloseReason` identifies `ScrimClick`, `EscapeKey`, `CloseButton`, `Programmatic`, or `Navigation`.
UI dismissal, the dialog's `CloseAsync`, service handles, and internal navigation all consult the
same context. A backdrop dismisses only when both press and release occur on that same backdrop;
dragging from the dialog onto it does not request closure.

Replace `FlareDialogInstance.Close(...)` / `Cancel()` and `DialogReference.Close(...)` / `Cancel()`
with `await CloseAsync(...)` / `await CancelAsync()`. They return `Task<bool>`: `true` means the close
was accepted, `false` means it was vetoed, another attempt was pending, or the dialog was already
closed. The first accepted result wins. Await the operation before assuming the dialog has closed.

Setting `Visible = false` directly is a forced state change and bypasses guards. Use the inline
dialog's `CloseAsync()` for user-triggered closure. A service dialog's successful result and its
cancel result are both guarded; clear the dirty state after saving before requesting either.

With `CloseOnNavigation = true`, internal navigation asks the guards before committing the new URL.
If another navigation handler cancels the transition, the dialog remains open. A reload, external
URL, or tab close cannot await a Blazor dialog: compose `FlareNavigationGuard IsDirty="_dirty"
GuardInAppNavigation="false"` for the browser's unload prompt. Its wording is controlled by the
browser. Set `CloseOnNavigation = false` if internal navigation should leave the dialog open.

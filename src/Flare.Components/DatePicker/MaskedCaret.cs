using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>
/// The caret of a masked date or time field: where it stood before the mask rewrote the text, and putting it back
/// afterwards, so an edit in the middle keeps the caret on the segment being typed (TASK-111).
/// </summary>
internal static class MaskedCaret
{
    /// <summary>Digits standing before the caret in the DOM's current (pre-mask) text; -1 when the selection is
    /// unavailable (tests, a lost connection, a non-text control).</summary>
    public static async Task<int> DigitsBeforeAsync(IElementJsService js, ElementReference input, string raw)
    {
        try
        {
            var selection = await js.GetSelectionAsync(input);
            if (selection is not { Length: 2 } || selection[0] < 0) return -1;
            return MaskedInput.DigitsBefore(raw, selection[0]);
        }
        catch (JSDisconnectedException) { return -1; }
        catch (JSException) { return -1; }
    }

    /// <summary>Writes the masked text and the caret back. The value is rewritten too: Blazor skips the DOM write when
    /// the masked text equals the previous one (a deleted separator), which would keep the half-edited text.</summary>
    public static async Task RestoreAsync(IElementJsService js, ElementReference input, string text, int caret)
    {
        try { await js.SetValueAndCaretAsync(input, text, caret, caret); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
    }
}

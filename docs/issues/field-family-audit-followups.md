# Field-family audit - follow-ups

**Source:** cross-framework audit of Flare's text/input FIELD family vs the locally-installed
MudBlazor / Blazorise / Fluent UI Blazor (2026-07-11). Components in scope: `FlareField<T>` /
`FlareTextField`, `FlarePasswordField`, `FlareNumericField<T>`, `FlareMaskedField`, `FlareTextArea`,
`FlareOtpField`. Shared plumbing: `FlareFieldBase`, `FlareTextInput`, `FlareFieldChrome`.

**Headline finding:** the whole button family gained `FocusAsync()`, but no field exposed any imperative
focus/select/blur method - the one capability all three competitors ship on every input. Flare already
leads on breadth (OTP exists in none of the three; Numeric focus-swap formatting + auto-mask + zero-JS
AutoGrow are strong).

Consensus gap ranking (how many of the 3 competitors have it):
`FocusAsync` 3/3, `Autofocus` 3/3, `Pattern` 3/3, `InputMode` 3/3, `OnBlur` 2/3, `SelectAsync` 2/3.

## Shipped on `main` (items 1-9)

1. **`FocusAsync()` across the editable field family** - new intermediate base
   `FlareEditableFieldBase` (`FocusAsync()` + `protected virtual FocusCoreAsync()`); every editable field
   (text, password, numeric, mask, textarea) plus the standalone `FlareOtpField` focuses its real control.
   Analog of the button-family `FocusAsync`. The select-family fields (select/multiselect/combobox/
   tagfield/date+time pickers) stay on `FlareFieldBase` and keep their own trigger/focus model - exposing
   a single-input focus API on a div-trigger select would be misleading, so it was deliberately not
   hoisted there (candidate for a separate select-family focus pass).
2. **`Autofocus`** parameter on `FlareEditableFieldBase` (focus on first render, best-effort) - inherited
   by every editable field. Also added directly to the standalone `FlareOtpField`.
3. **`Pattern`** (regex, HTML `pattern`) + **`InputMode`** surfaced on `FlareField`/`FlareTextField`
   (both were 3/3; `InputMode` already existed on the internal `FlareTextInput`, just not exposed).
4. **`OnFocus` / `OnBlur`** events on the text-entry fields (`FlareField`, `FlareTextArea`,
   `FlareMaskedField`, `FlareNumericField`; `FlarePasswordField` forwards).
5. **`SelectAsync()` / `BlurAsync()`** on the editable fields (select-all / programmatic blur, via a
   small `IElementJsService` addition). `FlareNumericField` too.
6. **`Clearable` on every editable field** - was only on `FlareField`; added to `FlareNumericField`,
   `FlareMaskedField`, `FlareTextArea` and forwarded from `FlarePasswordField`. + **`OnClearButtonClick`**.
7. **`FlareNumericField`**: public **`Increment()` / `Decrement()`** (were private `StepBy`) +
   **`SelectAllOnFocus`** (Blazorise parity).
8. **`Autocomplete` / `Spellcheck`** parameters on the text fields (Fluent parity; genuinely useful for
   password managers / 2FA autofill / disabling spellcheck on codes).
9. **`SelectRangeAsync(start, end)`** (caret range), **TextArea `Resize`** (None/Both/Vertical/Horizontal),
   **`DataList`** (native `<datalist>` suggestions), **`HelperTextOnFocus`** (show helper only while focused).

## Deferred

### 10 - `FlareOtpField` does not inherit `FlareFieldBase` - NEEDS DESIGN
`FlareOtpField` is a standalone `FlareComponentBase`, so it lacks the family chrome: no `Label`,
`HelperText`/`ErrorText`, `Variant`/`Size`, `Required`/`ReadOnly`, no `FlareFieldChrome` support row.
Open question: should OTP compose `FlareFieldChrome` (gaining label + helper/error + variant/size for
free) with its N-cell row in the `Field` slot, or stay deliberately standalone (its multi-input model is
unlike a single control - `Value` is a joined digit string, `ReadOnly` and floating labels are awkward)?
Decision pending. If adopted: OTP would render `<FlareFieldChrome>` with the cell row as `Field`,
inherit `FlareFieldBase` (Label/Helper/Error/Variant/Size/Required), and keep `Length`/`OnComplete`/
`Masked`/`AutoComplete`/`ClearAsync` as OTP-specifics. `FocusAsync`/`Autofocus` were added in items 1-2
regardless (they don't depend on the inheritance change).

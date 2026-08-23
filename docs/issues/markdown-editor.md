# Markdown: an edit mode, not only a renderer

**Status: OPEN. Phase 1, small.**

`FlareMarkdown` renders. `MarkdownParser` is a source-generated static parser, no JS, no Markdig
dependency - which is the right call and worth keeping. What is missing is authoring. Blazorise ships a
Markdown *editor* (`Blazorise.Markdown`, a wrapper over EasyMDE); Radzen renders only, like Flare;
MudBlazor and Fluent UI have neither.

Flare is unusually well placed here because it already owns the hard part twice over: `FlareCodeBlock`
implements the two-layer editing contract (an invisible `textarea` over a syntax-coloured `pre`) and
`FlareRichTextEditor` owns the toolbar model.

## The two-layer trap, restated

The `FlareCodeBlock` contract is load-bearing and easy to break: the transparent `textarea` and the
coloured `pre` beneath it must agree on **wrap mode, font, font-size, line-height, padding, tab size and
scroll offset**, or the caret lands on a different character than the one under the cursor. A `textarea`
soft-wraps by default and a `pre` does not - that mismatch alone is enough to desynchronise them, and
markdown is a wrapping-heavy format, so this editor stresses the contract far harder than code does.

Do not re-implement the overlay. Extract the shared behaviour out of `FlareCodeBlock` into an internal
overlay-editor primitive and have both consume it, so a fix to one is a fix to both.

## Scope

`FlareMarkdown` gains `Editable` and `Mode` (`Edit` / `Preview` / `Split`), `Value` / `ValueChanged`
two-way binding, and a toolbar that is opt-in and composable from the existing `FlareToolbar` and
`FlareIconButton` rather than a bespoke strip:

- bold, italic, strikethrough, inline code
- heading level cycle, quote, ordered and unordered list, task list
- link and image insertion (dialog through `IDialogService`, not `prompt()`)
- code fence with a language pick
- table insertion

Each toolbar action is a pure string transform over `(text, selectionStart, selectionEnd)` returning the
new text and new selection. That makes every action unit-testable with no browser, and it keeps the JS
surface at exactly one call - set the selection range - which goes through the existing
`IElementJsService` port rather than a new one.

Also worth having, cheaply: `AllowHtml` off by default (a markdown renderer that passes raw HTML through
is an XSS vector - validate at this boundary), drag-and-drop or paste of an image raised as an event so
the application decides where it is stored, and `Shortcuts` wired through `FlareShortcuts` for the usual
Ctrl+B / Ctrl+I / Ctrl+K.

Split mode is `FlareSplitter` with a scroll-sync flag - not a new layout.

## Tokens

A markdown token record does not exist yet; `markdown.css` is in the bundle. Introduce `MarkdownTokens`
covering both modes: heading typescales per level, code-span and code-fence background and radius,
blockquote bar color and width, table border and stripe, link color, list marker color and indent, plus
the editor-only surface - toolbar gap, active-mark background, and the split divider (reuse
`SplitterTokens`). `required`, no literals, as everywhere.

## Done when

- Round-trip is lossless: rendering the parse of a document and re-editing does not mutate the source.
- Caret and click land on the correct character at 80-column wrap in both themes, verified in a real
  browser at two font sizes - not from `getComputedStyle` in headless, which lies about frozen transitions.
- Every toolbar action has a unit test on the string transform, including the empty-selection case.
- `AllowHtml="false"` strips script and event-handler attributes; there is a test that proves it.
- Gallery page with edit / preview / split and a full document exercising every supported construct.

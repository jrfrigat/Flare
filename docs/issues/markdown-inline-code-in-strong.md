# Markdown: inline code inside bold is not parsed

**Status: OPEN. Small parser bug, visible on the Gallery's own /changelog page. Found 2026-09-03 while
verifying the 0.26.3 release; pre-existing, and every changelog entry written since at least 0.26.2
hits it.**

`MarkdownParser` renders inline code and strong emphasis, but not one inside the other. A bullet that
opens the way most CHANGELOG entries do:

```markdown
- **`FlareSelect` and `FlareMultiSelect` were shorter than every field beside them.** The combobox
  trigger set its own padding instead of the family's `--flare-input-padding-md`.
```

renders the second span as `<code>` and the first as literal backticks inside `<strong>`:

```html
<strong>`FlareSelect` and `FlareMultiSelect` were shorter than every field beside them.</strong>
```

Measured on 0.26.3 at `/changelog`: 250 `<code>` elements on the page, and zero of them inside a
`<strong>`; every `strong` whose source contained backticks shows them raw.

## Scope

The inline pass appears to resolve emphasis and code as alternatives rather than allowing code to open
inside an already-open emphasis run. Per CommonMark, code spans bind tighter than emphasis - a backtick
run inside `**...**` is a code span, and the emphasis wraps it.

Worth checking the other direction and the neighbours at the same time: bold inside a link, code inside a
link text, emphasis inside a list item's first word. Only the code-inside-bold case is confirmed.

## Why it matters more than it looks

This is the pattern the project's own release notes are written in, so the defect is on the page the
Gallery uses to present itself. It is also the pattern any technical documentation uses.

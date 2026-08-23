# Barcode: 1D symbologies beside the QR encoder

**Status: OPEN. Phase 1, small. Also carries the known QR version cap.**

`Flare.Components.QrCode` ships `FlareQrCode` over a self-written `QrCodeGenerator` - no JS, no external
encoder. Radzen and Blazorise both ship a 1D `Barcode` component in addition to their QR component;
MudBlazor and Fluent UI have neither. Warehouse, retail and logistics applications need Code 128 far more
often than they need QR, so this is a real gap and a cheap one - a 1D symbology is a much simpler encoder
than the QR one already written.

## Two things in one issue, because they touch the same encoder package

### 1. `FlareBarcode`

Symbologies, in order of how often they are actually asked for: **Code 128** (A/B/C with automatic
subset switching), **EAN-13** and **EAN-8**, **UPC-A**, **Code 39**, **ITF-14**, **Codabar**. Each is a
pure function from string to a bar-width array, so each is unit-testable against the published check
vectors without a browser.

Surface: `Value`, `Symbology`, `ShowText`, `TextPosition`, `Height`, `ModuleWidth`, `Quiet` (quiet-zone
modules), `Format` for the human-readable line, and `OnInvalid` for a value the symbology cannot encode -
which must not throw during render, because the value usually comes from a bound field the user is still
typing into.

Render as SVG rects, same approach as `FlareQrCode`, so it prints and scales.

### 2. Lift the QR version cap

Recorded already, restated here so it lands with this work: the QR encoder is capped at **version 4**,
which is roughly 50 alphanumeric characters at level M. A URL longer than that cannot be encoded at all.
Versions up to 40 need the remaining alignment-pattern table and the block-splitting rules for the higher
versions - mechanical work against the specification, well covered by public test vectors.

## Package shape

Rename is warranted: `Flare.Components.QrCode` becomes `Flare.Components.Barcode` holding both, or a new
sibling package is added. Prefer **one package** - they share the module-to-SVG rendering path, the quiet
zone handling, the token record and the CSS file, and two packages would duplicate all four. Keep the old
package id publishing as a dependency-only shim for one release so existing consumers do not break.

## Tokens

Extend the existing QR token record rather than adding a second: module color, background, quiet-zone
color, text typescale / color / gap, and the corner radius applied to modules (some themes soften QR
modules; barcode bars stay square). `required`, no literals.

## Done when

- Code 128, EAN-13, EAN-8, UPC-A, Code 39 encode correctly against published check vectors in unit tests.
- An invalid value renders an empty, accessible placeholder instead of throwing.
- QR encodes at least a 300-character URL (version 40, level L) and the existing tests still pass.
- Both components render identically in print and at 4x zoom - no rasterization anywhere.
- Gallery demo shows a scannable barcode and a scannable long-URL QR side by side.

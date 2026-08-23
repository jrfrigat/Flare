# Integration satellites: map, PDF viewer, cropper, chat

**Status: OPEN. Phase 4, medium each. Independent - none blocks another.**

Four components that exist in the reference frameworks and that Flare lacks. They are grouped because
they share one architectural problem: **each of them is provider-shaped.** A map is Google or Leaflet or
Azure; a PDF viewer is pdf.js or a native embed; a cropper and a chat surface both reach for browser APIs
or a backend service.

Radzen solved this by shipping `RadzenGoogleMap` - a component named after a vendor. Blazorise ships
`Blazorise.Maps`, `Blazorise.PdfViewer` and `Blazorise.Cropper` as wrappers over specific JS libraries.
Both approaches put a vendor into the dependency graph of a UI library.

**Flare's constraint makes this straightforward:** third-party SDKs may not enter `Flare.Components`, and
service implementations belong in `Flare.Infrastructure`. So every one of these is the same shape:

- a **port** in `Flare.Abstractions` describing what the component needs, in the component's own vocabulary
- a **component** in a satellite package that talks only to the port and renders only tokens
- an **adapter** shipped separately (or written by the application) that binds the port to a provider

That means Flare ships `FlareMap`, not `FlareGoogleMap`, and an application picks its provider. This is
strictly better than what all four reference frameworks do, and it is the reason these are worth building
rather than telling users to drop in a `<div>`.

---

## 1. `Flare.Components.Map`

Port: `IMapProvider` - initialize on an element, set center and zoom, add/remove/update markers and
shapes, geocode optionally, raise click and bounds-changed. The component owns markers, clusters, popups
(reusing `FlarePopover`), the control chrome and the token surface; the provider owns tiles and
projection. Ship one reference adapter over an open provider (Leaflet/OSM) in a separate package so the
default path has no API key and no commercial terms.

Radzen has Google only; Blazorise wraps Google and MapLibre. A provider-agnostic map is a differentiator.

## 2. `Flare.Components.PdfViewer`

Port: `IPdfRenderer` - open a document from bytes or URL, page count, render a page to a canvas or image
at a scale, extract text for search. The component owns pagination, zoom, rotation, thumbnails, the search
UI, and the toolbar (from `FlareToolbar`).

The reference adapter wraps pdf.js. Note the constraint already known about this codebase: JS assets under
`_content` are unfingerprinted and a service worker will serve a stale copy, so any new C#-to-JS call in
this package must tolerate `JSException` from an older script.

Blazorise has a viewer; nobody else does.

## 3. `Flare.Components.Cropper`

Blazorise has `Cropper`; nobody else does. It pairs directly with the upload work in Phase 1 - crop before
transfer is the actual workflow.

This one may not need a provider at all: pointer-driven crop box over an `<img>`, aspect-ratio locking,
rotate and flip, zoom, and output through a canvas. The only genuinely browser-side part is producing the
cropped bytes, which is one call behind a port - `IImageCanvas` - and `IColorCanvasJsService` already
establishes that pattern in `Flare.Infrastructure`.

Crop box interaction reuses the pointer-drag primitive from the Scheduler and TileLayout work.

## 4. `Flare.Components.Chat`

Radzen has `RadzenChat`, `RadzenAIChat` and `RadzenSpeechToTextButton`. This is the newest category and
the one most likely to be asked for by name.

The component is a **transcript surface**, not an AI client: message list with author, timestamp, status
and grouping; streaming partial messages; markdown rendering through `FlareMarkdown`; attachments through
the upload work; a composer with send-on-enter, multiline and attachment affordances; typing indicators;
scroll-anchoring that holds position when history loads above.

The port is `IChatTransport` - send a message, subscribe to a stream of deltas. Whether the other end is
a model API, SignalR or a support desk is not the component's business, and Flare must not take a
dependency on any AI SDK. Speech input is a separate small port over the browser speech API, used by an
optional composer button.

Do this one last of the four unless it is asked for: it is the most fashionable and the least structural.

---

## Common definition of done

- Nothing in `Flare.Components` or the satellite package references a vendor SDK; the port carries no
  provider-specific type in its signature.
- The component renders its empty and error states unstyled without a theme, and fully themed with each
  shipped theme.
- Reference adapter lives in its own package with its own README and is genuinely optional.
- Gallery demo runs on the reference adapter without secrets checked into the repository.

# FileUpload: it selects files, it does not upload them

**Status: OPEN. Phase 1. The most visible hole in a component Flare already ships.**

`FlareFileUploadZone` and `FlareFileUploadButton` look like upload components and are named like upload
components. They are file *pickers*. The whole parameter surface of
`src/Flare.Components/FileUpload/FlareFileUploadBase.cs` is:

```
OnFilesChanged(IReadOnlyList<IBrowserFile>)   Accept   Multiple   Disabled
MaxFiles (10)   MaxFileSize (long.MaxValue)   ShowFileList (true)
```

There is no `HttpClient`, no URL, no progress, no cancel, no retry, no per-file state - `grep` for
`HttpClient|UploadUrl|PostAsync` under `FileUpload/` returns nothing. Every application using Flare has
to write the transfer loop itself, including the progress UI, which means it also has to write the
styling for a progress UI that has no tokens.

Both competitors that take this seriously ship the transfer: Radzen's `RadzenUpload` posts to `Url` with
`Progress` / `Complete` / `Error` events and per-request headers via `RadzenUploadHeader`; Blazorise's
`FilePicker` and `FileEdit` do chunked streaming with progress and auto-upload. MudBlazor and Fluent UI
are in the same place Flare is - selection only - so this is a chance to lead, not to catch up.

## Re-specified: Flare owns the state machine, the application owns the bytes

The plan below was written `Url`-first - the component takes a URL, headers and form fields, and Flare
does the POST. That is what `RadzenUpload` does, and it is the reason people end up wrapping it. It
breaks on the cases real applications actually have:

- a bearer token that refreshes **mid-upload**, so a header captured at render time is already stale;
- a **presigned S3/Azure URL** - a `PUT` of the raw body, not a multipart `POST` with a field name;
- **resumable/chunked protocols** (tus and friends) that negotiate an offset before sending;
- request signing, per-tenant endpoints, a retry policy the application already owns for every other
  call it makes.

None of that is expressible as "a URL and a dictionary of headers", and chasing it would grow an HTTP
client inside Flare that is still wrong for the next application.

**Split it where the value actually is.** What every application rewrites today is not the `POST` - it
is the per-file state machine and the UI on top of it: queued / uploading / done / failed / cancelled,
a progress bar with no tokens, cancel and retry affordances, an error row. That is Flare's job, and it
is the half that has to be tokenized.

So the component's contract is a delegate, not a URL:

```csharp
[Parameter] public Func<FlareUploadContext, Task>? Uploader { get; set; }

public readonly record struct FlareUploadContext(
    IBrowserFile File, IProgress<long> Progress, CancellationToken CancellationToken);
```

The application sends the bytes however it likes and reports progress back; Flare drives the queue,
concurrency, cancellation and the whole visual state. Anything the application can write with an
`HttpClient` works - including all four cases above - and Flare cannot be wrong about its auth.

The `Url` convenience does not disappear, it just stops being the contract. `IFlareUpload` in
`Abstractions` with an `HttpClient` implementation in `Infrastructure` hands back a ready-made delegate
for the simple case:

```razor
<FlareFileUploadZone Uploader="@Upload.To("/api/files")" />
```

That keeps roadmap rule 5 (the implementation is in `Infrastructure`, never in `Flare.Components`) while
making the simple case one line and the hard cases possible at all.

## Why the transfer helper is a port, not a component change

Rule 5 of the roadmap. The transfer needs `HttpClient`, cancellation, retry policy and possibly chunking:
none of that belongs in `Flare.Components`, which must not gain service implementations. The shape
already exists in the codebase and should be mirrored exactly - `IFlareDownload` in
`Flare.Abstractions/Abstractions/` with `FlareDownloadService` in `Flare.Infrastructure/JsInterop/`.

**Add `IFlareUpload` beside `IFlareDownload`**, implemented in `Flare.Infrastructure`, registered by
`AddFlare` in the composition root. Sketch:

```csharp
public interface IFlareUpload
{
    ValueTask<FlareUploadResult> UploadAsync(
        IBrowserFile file,
        FlareUploadRequest request,
        IProgress<FlareUploadProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
```

with `FlareUploadRequest` carrying the URL, method, form field name, extra form fields and headers;
`FlareUploadProgress` carrying bytes-sent / total / percent; and `FlareUploadResult` carrying status,
response body and error. Records, not classes with setters, to match the rest of `Abstractions`.

The component consumes the port only through a cascaded or injected reference held by the *root*
package - `Flare.Components` may reference the interface (it lives in `Abstractions`), never the
implementation. That is the same arrangement `FlareDataGrid` export already uses.

## Component surface to add

On `FlareFileUploadBase`, so both the zone and the button inherit it:

| Parameter | Purpose |
| :-- | :-- |
| `Uploader` | The transfer itself, supplied by the application. Absent it, the component behaves exactly as today - selection only, fully backward compatible. |
| `Auto` | Start on selection instead of waiting for `UploadAsync()`. |
| `Concurrency` | Parallel file limit; default 1, because servers hate surprises. |
| `ChunkSize` | Optional chunked transfer for large files. |
| `OnUploadStarted`, `OnUploadProgress`, `OnUploadCompleted`, `OnUploadFailed`, `OnAllCompleted` | Event surface. `OnFilesChanged` keeps its current meaning. |
| `AllowCancel`, `AllowRetry`, `AllowRemove` | Per-file affordances in the file list. |
| `FileTemplate` | `RenderFragment<FlareUploadFile>` so an application can replace the row wholly. |

A public `UploadAsync()` / `CancelAsync(fileId)` method pair on the component for imperative control.

State per file - queued / uploading / done / failed / cancelled - is a `FlareUploadFile` record exposed
through the template context, so applications can drive their own list from the same model.

## Tokens

`FileUploadTokens.cs` exists and needs extending, `required`, no literals: progress track and indicator
color and height, per-state row background and foreground (queued / active / success / error), the
dropzone active and reject states, and the row gap and radius. Reuse `ProgressTokens` for the bar itself
rather than inventing a second progress bar - roadmap rule 3.

## Done when

- `IFlareUpload` in `Abstractions`, implementation in `Infrastructure`, wired in `AddFlare`.
- A file uploads to a real endpoint from the Gallery demo with visible per-file progress, cancel and retry.
- Cancellation actually aborts the request, verified from the network panel, not just the UI state.
- Failure path renders through tokens - no hardcoded red.
- Existing usage without `Url` behaves byte-identically to today; the current bUnit tests pass unchanged.
- New strings ("Cancel", "Retry", "Remove", "%1 of %2") localized in both resx files.

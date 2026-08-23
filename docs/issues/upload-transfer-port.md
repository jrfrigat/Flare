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

## Why this is a port, not a component change

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
| `Url`, `Method`, `FieldName`, `Headers`, `FormFields` | Where and how to send. Absent `Url`, the component behaves exactly as today - selection only, fully backward compatible. |
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

namespace Flare.Components;

/// <summary>
/// The convenience half of the upload story: a ready-made transfer for the simple case, so an
/// application that just posts a file to its own endpoint does not have to write one.
/// </summary>
/// <remarks>
/// It hands back a delegate rather than uploading directly, because the delegate is what the component
/// takes. That keeps one contract - <c>FlareFileUploadZone.Uploader</c> - whether the transfer came from
/// here or from the application, and it means an application can start with <see cref="To"/> and replace
/// it later with its own without touching the markup.
/// <para>
/// This is deliberately the small case. Anything with a token that refreshes mid-upload, a presigned
/// <c>PUT</c>, a resumable protocol or an existing retry policy should write the delegate itself; that is
/// the whole reason the component's contract is a delegate.
/// </para>
/// </remarks>
public interface IFlareUpload
{
    /// <summary>
    /// Builds a transfer that posts one file as <c>multipart/form-data</c>.
    /// </summary>
    /// <param name="url">Where to post.</param>
    /// <param name="fieldName">The form field the file arrives under. Defaults to <c>file</c>.</param>
    /// <param name="headers">Extra request headers - an API key, a correlation id.</param>
    /// <param name="fields">Extra form fields sent alongside the file.</param>
    /// <returns>A delegate to hand to <c>Uploader</c>.</returns>
    Func<FlareUploadContext, Task> To(
        string url,
        string fieldName = "file",
        IReadOnlyDictionary<string, string>? headers = null,
        IReadOnlyDictionary<string, string>? fields = null);
}

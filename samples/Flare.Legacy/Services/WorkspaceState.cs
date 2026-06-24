using Flare.Legacy.Data;

namespace Flare.Legacy.Services;

/// <summary>Whether an open document tab shows a loan card or a client card.</summary>
public enum DocKind
{
    /// <summary>A loan (contract) detail card.</summary>
    Loan,
    /// <summary>A client card with the client's contracts.</summary>
    Client,
}

/// <summary>An open document tab.</summary>
/// <param name="Kind">Loan or client card.</param>
/// <param name="Key">Stable identity used to de-duplicate tabs (e.g. <c>L:10000001</c>).</param>
/// <param name="Label">The tab caption.</param>
/// <param name="Loan">The loan row (loan tabs only).</param>
/// <param name="ClientId">The client id (client tabs only).</param>
public sealed record OpenDoc(DocKind Kind, string Key, string Label, LoanRow? Loan, string? ClientId);

/// <summary>Holds the open loan/client tabs of the loans workspace. Registered scoped (one per app
/// session in WebAssembly), so the open tabs survive navigation to other pages and back.</summary>
public sealed class WorkspaceState
{
    private readonly List<OpenDoc> _docs = [];

    /// <summary>The open document tabs, in order (after the pinned list tab).</summary>
    public IReadOnlyList<OpenDoc> Docs => _docs;

    /// <summary>The active tab index (0 = the pinned list tab).</summary>
    public int ActiveIndex { get; set; }

    /// <summary>Raised whenever the open tabs change; subscribers call StateHasChanged.</summary>
    public event Action? Changed;

    /// <summary>Opens (or re-activates) a loan in its own tab.</summary>
    public void OpenLoan(LoanRow row)
        => Open(new OpenDoc(DocKind.Loan, $"L:{row.LoanId}", $"Займ {ShortName(row.ClientName)}", row, null));

    /// <summary>Opens (or re-activates) a client card in its own tab.</summary>
    public void OpenClient(LoanRow row)
        => Open(new OpenDoc(DocKind.Client, $"C:{row.ClientId}", $"Клиент {ShortName(row.ClientName)}", null, row.ClientId));

    private void Open(OpenDoc doc)
    {
        var existing = _docs.FindIndex(d => d.Key == doc.Key);
        ActiveIndex = existing >= 0 ? existing + 1 : AddAndIndex(doc); // +1 for the pinned list tab at 0
        Changed?.Invoke();
    }

    private int AddAndIndex(OpenDoc doc)
    {
        _docs.Add(doc);
        return _docs.Count; // the new last tab
    }

    /// <summary>Closes the document tab with the given caption (only document tabs are closeable).</summary>
    public void CloseByLabel(string title)
    {
        var idx = _docs.FindIndex(d => d.Label == title);
        if (idx < 0) return;
        _docs.RemoveAt(idx);
        // Closed tab was at index idx + 1; keep a valid active tab.
        ActiveIndex = Math.Clamp(ActiveIndex > idx ? ActiveIndex - 1 : ActiveIndex, 0, _docs.Count);
        Changed?.Invoke();
    }

    // "Иванова Анна Сергеевна" -> "Иванова А.С."
    private static string ShortName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return fullName;
        var initials = string.Concat(parts.Skip(1).Select(p => $"{p[0]}."));
        return parts.Length > 1 ? $"{parts[0]} {initials}" : parts[0];
    }
}

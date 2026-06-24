using Flare.Components;
using Flare.Legacy.Data;

namespace Flare.Legacy.Services;

/// <summary>A fake "backend": every call is delayed by <see cref="AppSettings.BackendDelayMs"/> so the
/// UI shows its loading state. Applies the grid's sort/filter/quick-search/paging request in memory.</summary>
public sealed class LoanBackend(AppSettings settings)
{
    /// <summary>Awaits the configured simulated latency.</summary>
    public Task DelayAsync() => Task.Delay(Math.Max(0, settings.BackendDelayMs));

    /// <summary>Loads a single loan (joined with its client) by contract id, with latency.</summary>
    public async Task<LoanRow?> GetLoanAsync(string loanId)
    {
        await DelayAsync();
        var loan = LoanData.Loans.FirstOrDefault(l => l.LoanId == loanId);
        return loan is null ? null : LoanData.ToRow(loan);
    }

    /// <summary>Loads a client and the list of their contracts, with latency.</summary>
    public async Task<(Client Client, IReadOnlyList<LoanRow> Loans)?> GetClientAsync(string clientId)
    {
        await DelayAsync();
        var client = LoanData.Client(clientId);
        if (client is null) return null;
        var rows = LoanData.Loans.Where(l => l.ClientId == clientId).Select(LoanData.ToRow).ToList();
        return (client, rows);
    }

    /// <summary>Server-side items provider for the loans grid: applies quick search, column filters,
    /// the advanced filter tree, sort and paging.</summary>
    public async Task<DataGridResult<LoanRow>> QueryAsync(DataGridRequest req)
    {
        await DelayAsync();

        IEnumerable<LoanRow> q = LoanData.Rows();

        // Global quick filter: matches contract id/number, client id, or full name.
        if (!string.IsNullOrWhiteSpace(req.QuickFilter))
        {
            var needle = req.QuickFilter.Trim();
            q = q.Where(r =>
                r.LoanId.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                r.LoanNumber.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                r.ClientId.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                r.ClientName.Contains(needle, StringComparison.OrdinalIgnoreCase));
        }

        // Column filters (text Contains, or multi-select Values).
        foreach (var f in req.FilterModel)
        {
            var sel = Selector(f.Key);
            if (sel is null) continue;
            if (f.Values is { Count: > 0 } values)
                q = q.Where(r => values.Contains(sel(r)));
            else if (!string.IsNullOrWhiteSpace(f.Value))
                q = q.Where(r => sel(r).Contains(f.Value, StringComparison.OrdinalIgnoreCase));
        }

        // Advanced filter builder tree (AND/OR of Equals/Contains conditions).
        if (req.FilterTree is { IsEmpty: false } tree)
            q = q.Where(r => MatchGroup(tree, r));

        // Sort (first sort instruction).
        if (req.Sorts.FirstOrDefault() is { } sort && Selector(sort.Key) is { } ssel)
            q = sort.Direction == SortDirection.Descending ? q.OrderByDescending(ssel) : q.OrderBy(ssel);

        var all = q.ToList();
        var page = all.Skip(req.Page * req.PageSize).Take(req.PageSize);
        return new DataGridResult<LoanRow>(page, all.Count);
    }

    private static Func<LoanRow, string>? Selector(string key) => key switch
    {
        "ID договора" => r => r.LoanId,
        "Номер договора" => r => r.LoanNumber,
        "ID клиента" => r => r.ClientId,
        "ФИО" => r => r.ClientName,
        "Стадия" => r => r.Stage,
        "Остаток" => r => r.Balance,
        "Просрочка" => r => r.Overdue,
        _ => null,
    };

    private static bool MatchGroup(DataGridFilterGroup g, LoanRow r)
    {
        var conds = g.Conditions.Select(c => MatchCondition(c, r));
        var groups = g.Groups.Select(sub => MatchGroup(sub, r));
        var all = conds.Concat(groups).ToList();
        if (all.Count == 0) return true;
        return g.Or ? all.Any(x => x) : all.All(x => x);
    }

    private static bool MatchCondition(DataGridFilter c, LoanRow r)
    {
        var sel = Selector(c.Key);
        if (sel is null) return true;
        var v = sel(r);
        var operand = c.Value ?? "";
        return c.Operator switch
        {
            FilterOperator.Equals => string.Equals(v, operand, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotEquals => !string.Equals(v, operand, StringComparison.OrdinalIgnoreCase),
            FilterOperator.StartsWith => v.StartsWith(operand, StringComparison.OrdinalIgnoreCase),
            FilterOperator.EndsWith => v.EndsWith(operand, StringComparison.OrdinalIgnoreCase),
            _ => v.Contains(operand, StringComparison.OrdinalIgnoreCase),
        };
    }
}

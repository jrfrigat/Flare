using System.Globalization;

namespace Flare.Legacy.Data;

/// <summary>A client (borrower). One client may hold several loan contracts.</summary>
public sealed record Client(
    string ClientId,
    string Name,
    string BirthDate,
    string Income,
    string Status,
    string Phone);

/// <summary>A single loan contract that belongs to a <see cref="Client"/>.</summary>
public sealed record Loan(
    string LoanId,
    string LoanNumber,
    string ClientId,
    string Stage,
    string Balance,
    string Overdue);

/// <summary>A flattened grid row: a loan joined with its client's display fields. Used by the
/// "Список займов" grid, the loan detail tab, and the client card's contract list.</summary>
public sealed record LoanRow(
    string LoanId,
    string LoanNumber,
    string ClientId,
    string ClientName,
    string BirthDate,
    string Income,
    string Stage,
    string Balance,
    string Overdue);

/// <summary>The fictional demo portfolio: 10 clients, each holding 5..20 loan contracts.</summary>
public static class LoanData
{
    /// <summary>All clients, keyed by id for quick joins.</summary>
    public static readonly IReadOnlyList<Client> Clients;

    /// <summary>All loan contracts across every client.</summary>
    public static readonly IReadOnlyList<Loan> Loans;

    private static readonly Dictionary<string, Client> _byId;

    static LoanData()
    {
        var rnd = new Random(20260622); // deterministic demo data
        var clients = new List<Client>();
        var loans = new List<Loan>();
        var loanSeq = 10000001;

        for (var i = 0; i < _names.Length; i++)
        {
            var clientId = (5965000 + i * 137).ToString(CultureInfo.InvariantCulture);
            clients.Add(new Client(
                ClientId: clientId,
                Name: _names[i],
                BirthDate: _birthDates[i],
                Income: Money(25000 + rnd.Next(0, 60) * 1000),
                Status: _statuses[rnd.Next(_statuses.Length)],
                Phone: $"+7 9{rnd.Next(10, 99)} {rnd.Next(100, 999)}-{rnd.Next(10, 99)}-{rnd.Next(10, 99)}"));

            var count = rnd.Next(5, 21); // 5..20 contracts
            for (var j = 0; j < count; j++)
            {
                var loanId = loanSeq++.ToString(CultureInfo.InvariantCulture);
                var balance = rnd.Next(2000, 50000) + rnd.NextDouble();
                var overdue = rnd.Next(0, 4) == 0 ? 0 : rnd.Next(100, 13000) + rnd.NextDouble();
                loans.Add(new Loan(
                    LoanId: loanId,
                    LoanNumber: $"{rnd.Next(0, 9)} {rnd.Next(0, 999):000} {rnd.Next(10, 99)} {rnd.Next(0, 9)} / 2026",
                    ClientId: clientId,
                    Stage: _stages[rnd.Next(_stages.Length)],
                    Balance: Money((decimal)balance),
                    Overdue: overdue == 0 ? "0,00" : Money((decimal)overdue)));
            }
        }

        Clients = clients;
        Loans = loans;
        _byId = clients.ToDictionary(c => c.ClientId);
    }

    /// <summary>Every loan flattened with its client's name and personal fields.</summary>
    public static IEnumerable<LoanRow> Rows() => Loans.Select(ToRow);

    /// <summary>Joins a loan with its client into a display row.</summary>
    public static LoanRow ToRow(Loan l)
    {
        var c = _byId[l.ClientId];
        return new LoanRow(l.LoanId, l.LoanNumber, c.ClientId, c.Name, c.BirthDate, c.Income, l.Stage, l.Balance, l.Overdue);
    }

    /// <summary>The client with the given id, or null.</summary>
    public static Client? Client(string clientId) => _byId.GetValueOrDefault(clientId);

    // Format a number RU-style ("13 997,45") without depending on culture data in WASM.
    private static string Money(decimal v)
        => v.ToString("N2", CultureInfo.InvariantCulture).Replace(",", " ").Replace('.', ',');

    private static readonly string[] _stages =
        ["Выдан", "Soft", "Hard", "Раннее взыскание", "Реструктуризация"];

    private static readonly string[] _statuses = ["Обычный", "VIP", "Проблемный", "Новый"];

    private static readonly string[] _names =
    [
        "Иванова Анна Сергеевна",
        "Петров Игорь Львович",
        "Сидорова Мария Ивановна",
        "Кузнецов Олег Павлович",
        "Морозова Елена Викторовна",
        "Васильев Денис Юрьевич",
        "Смирнова Ольга Дмитриевна",
        "Фёдоров Артём Николаевич",
        "Николаева Татьяна Олеговна",
        "Захаров Павел Андреевич",
    ];

    private static readonly string[] _birthDates =
    [
        "14.03.1990", "02.11.1985", "21.07.1978", "30.01.1995", "09.09.1982",
        "17.05.1991", "25.12.1988", "08.06.1979", "11.02.1993", "03.08.1986",
    ];
}

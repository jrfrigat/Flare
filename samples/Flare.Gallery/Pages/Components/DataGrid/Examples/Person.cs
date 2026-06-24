namespace Flare.Gallery.Pages.Components.DataGrid.Examples;

internal sealed class Person
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string City { get; set; } = "";
    public int Score { get; set; }
}

internal static class DatagridData
{
    internal static readonly List<Person> _people =
    [
        new() { Name = "Alice Johnson",   Role = "Engineer",  City = "Berlin",     Score = 92 },
            new() { Name = "Bob Smith",       Role = "Designer",  City = "London",     Score = 78 },
            new() { Name = "Carol Williams",  Role = "Manager",   City = "Paris",      Score = 85 },
            new() { Name = "David Lee",       Role = "Engineer",  City = "Tokyo",      Score = 91 },
            new() { Name = "Emma Davis",      Role = "QA",        City = "New York",   Score = 74 },
            new() { Name = "Frank Miller",    Role = "DevOps",    City = "Amsterdam",  Score = 88 },
            new() { Name = "Grace Wilson",    Role = "Designer",  City = "Barcelona",  Score = 82 },
            new() { Name = "Henry Taylor",    Role = "Engineer",  City = "Berlin",     Score = 95 },
            new() { Name = "Iris Anderson",   Role = "Manager",   City = "London",     Score = 79 },
            new() { Name = "Jack Thomas",     Role = "Engineer",  City = "Singapore",  Score = 87 },
            new() { Name = "Karen Jackson",   Role = "QA",        City = "Toronto",    Score = 71 },
            new() { Name = "Liam Martinez",   Role = "DevOps",    City = "Madrid",     Score = 90 },
        ];
}
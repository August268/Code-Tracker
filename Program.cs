using System.Configuration;
using Microsoft.Data.Sqlite;
using Spectre.Console;

// Geting connection string from App.config
string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
bool closeApp = false;

// Create database and coding sessions table if not exist
using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    var tableCmd = connection.CreateCommand();

    tableCmd.CommandText =
        @"CREATE TABLE IF NOT EXISTS coding_sessions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            StartTime TEXT,
            EndTime TEXT,
            Duration REAL
            )";

    tableCmd.ExecuteNonQuery();

    connection.Close();
}

// Main Menu
Console.Clear();

while (!closeApp){
    var selectedOption = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Please enter from 1-4 to choose the options below:")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more options.)[/]")
            .AddChoices(new[] {
                "1", "2", "3", "4", "5", "6", "7"
            }));

    switch (selectedOption)
    {
        case "1":
            break;
        case "2":
            break;
        case "7":
            Console.Clear();
            var panel = new Panel("Goodbye...")
                .Header(new PanelHeader("Notice"))
                .Border(BoxBorder.Double)
                .Padding(new Padding(2, 2, 2, 2))
            AnsiConsole.Write(panel);
            closeApp = true;
            break;
        
        default:
        Console.Clear();
            Console.WriteLine("Invalid input");
            Console.ReadKey();
            break;
    }
};



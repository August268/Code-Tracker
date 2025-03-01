using System.Configuration;
using Code_Tracker;
using Microsoft.Data.Sqlite;
using Spectre.Console;

// Geting connection string from App.config
string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

UserInput input = new UserInput();

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

input.ShowMainMenu();



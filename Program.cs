using System.Configuration;
using Code_Tracker;
using System.Data.SQLite;
using Dapper;
using System.Data;
using Spectre.Console;

// Geting connection string from App.config
string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

UserInput input = new UserInput();

// Connect to database (Create one if it doesn't exist) using Dapper
using (IDbConnection connection = new SQLiteConnection(_connectionString))
{
    connection.Open();

    // Define the SQL to create a table if it doesn't exists
    string createTableSql = @"
        CREATE TABLE IF NOT EXISTS CodingSessions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            StartTime TEXT NOT NULL,
            EndTime TEXT NOT NULL,
            Duration REAL)";

    // Execute the SQL to create the table
    connection.Execute(createTableSql);

}

input.ShowMainMenu();

using System.Configuration;
using System.Collections.Specialized;
using Spectre.Console;
using Microsoft.Data.Sqlite;
using System.Globalization;


namespace Code_Tracker
{
    public class CodingSessionController
    {
        readonly string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        public void GetAllSessions()
        {
            AnsiConsole.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"SELECT * FROM coding_sessions";

                List<CodingSession> tableData = [];

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new CodingSession
                            {
                                Id = reader.GetInt32(0),
                                StartTime = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy HH-mm-ss", new CultureInfo("en-US")),
                                EndTime = DateTime.ParseExact(reader.GetString(2), "dd-MM-yyyy HH-mm-ss", new CultureInfo("en-US")),
                                Duration = reader.GetFloat(3),
                            }
                        );
                    }
                } else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                foreach (var sessions in tableData)
                {
                    Console.WriteLine($"{sessions.Id} - Start: {sessions.StartTime} - End: {sessions.EndTime} - Duration: {sessions.Duration}");
                }
                AnyKeyPrompt();
            }
        }

        public void CreateSession()
        {
            AnsiConsole.Clear();

            var header = new Panel(
                    Align.Center(
                        new Markup("[blue]CREATING SESSION[/]"),
                        VerticalAlignment.Middle
                    )
                ).Border(BoxBorder.Heavy).BorderStyle(Color.Blue);

            AnsiConsole.Write(header);
            AnsiConsole.WriteLine();


            string StartTime = GetUserInput("Start Time (Format: dd-MM-yyyy HH-mm-ss)", "Please enter start date and time: ");

            
        }

        // private CodingSession GetCodingSession()
        // {
        //     CodingSession cs = new();

        //     var 
        // }


        // Templates
        private string GetUserInput(string header, string prompt)
        {
            var inputRule = new Rule(header).Border(BoxBorder.Rounded).LeftJustified();
            AnsiConsole.Write(inputRule);
            AnsiConsole.WriteLine();
            string input = AnsiConsole.Ask<string>($"{prompt}: ");

            return input;
        }
        private void AnyKeyPrompt()
        {
            var rule = new Rule().Border(BoxBorder.Double);
            AnsiConsole.Write(rule);
            AnsiConsole.Write("Press any key to continue.");
            AnsiConsole.Record();
            AnsiConsole.Clear();
        }
    }
}
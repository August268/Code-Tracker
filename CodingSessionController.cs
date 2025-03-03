using System.Configuration;
using System.Collections.Specialized;
using Spectre.Console;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using Dapper;


namespace Code_Tracker
{
    public class CodingSessionController
    {
        readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        public void GetAllSessions()
        {
            AnsiConsole.Clear();

            using (IDbConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM CodingSessions";

                var sessionData = connection.Query<CodingSession>(query);

                connection.Close();

                if (sessionData.Count() == 0)
                {
                    AnsiConsole.WriteLine("No sessions are found...");
                }
                else
                {
                    foreach (var sessions in sessionData)
                    {
                        Console.WriteLine($"{sessions.Id} - Start: {sessions.StartTime} - End: {sessions.EndTime} - Duration: {sessions.Duration}");
                    }
                }
            }

            AnyKeyPrompt();
        }

        // public void CreateSession()
        // {
        //     AnsiConsole.Clear();

        //     var header = new Panel(
        //             Align.Center(
        //                 new Markup("[blue]CREATING SESSION[/]"),
        //                 VerticalAlignment.Middle
        //             )
        //         ).Border(BoxBorder.Heavy).BorderStyle(Color.Blue);

        //     AnsiConsole.Write(header);
        //     AnsiConsole.WriteLine();


        //     string StartTime = GetUserInput("Start Time (Format: dd-MM-yyyy HH-mm-ss)", "Please enter start date and time: ");


        // }

        // // private CodingSession GetCodingSession()
        // // {
        // //     CodingSession cs = new();

        // //     var 
        // // }


        // Templates
        private string GetUserInput(string header, string prompt)
        {
            var inputRule = new Spectre.Console.Rule(header).Border(BoxBorder.Rounded).LeftJustified();
            AnsiConsole.Write(inputRule);
            AnsiConsole.WriteLine();
            string input = AnsiConsole.Ask<string>($"{prompt}: ");

            return input;
        }
        private void AnyKeyPrompt()
        {
            var rule = new Spectre.Console.Rule().Border(BoxBorder.Double);
            AnsiConsole.Write(rule);
            AnsiConsole.Write("Press any key to continue.");
            Console.ReadKey();
            AnsiConsole.Clear();
        }
    }
}
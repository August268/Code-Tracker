using System.Configuration;
using Spectre.Console;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using Dapper;


namespace Code_Tracker
{
    public class CodingSessionController
    {
        // Used as a state for coding sessions when creating new ones
        Dictionary<string, string> sessionState = new Dictionary<string, string> {{ "startTime", "" }, { "endTime", "" }};
        readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        public void GetAllSessions()
        {
            AnsiConsole.Clear();

            using (IDbConnection connection = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT * FROM CodingSessions";

                var sessionData = connection.Query<CodingSession>(query).ToList();

                if (sessionData.Count() == 0)
                {
                    AnsiConsole.WriteLine("No sessions are found...");
                }
                else
                {
                    foreach (var sessions in sessionData)
                    {
                        Console.WriteLine($"{sessions.Id} - Start: {sessions.StartTime} - End: {sessions.EndTime} - Duration: {sessions.Duration} hours");
                    }
                }
            }

            AnyKeyPrompt();
        }

        public void CreateSession()
        {
            bool isValidStartTime = false;
            bool isValidEndTime = false;

            // Handles user input
            while (!isValidStartTime | !isValidEndTime)
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

                // Handling start time input
                if (sessionState["startTime"] == "")
                {
                    string startTime = GetUserInput("Start Time (Format: dd-MM-yyyy HH:mm:ss)", "Please enter start date and time");

                    // Exit if input is "0"
                    // Otherwise check if input is valid
                    if (startTime == "0")
                    {
                        ResetState();
                        break;
                    }
                    else if (!DateTime.TryParseExact(startTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        sessionState["startTime"] = "";
                        InvalidInputNotice();
                        continue;
                    }
                    else
                    {
                        isValidStartTime = true;
                        sessionState["startTime"] = startTime;
                    }
                }
                else
                {
                    var inputRule = new Spectre.Console.Rule("Start Time (Format: dd-MM-yyyy HH:mm:ss)").Border(BoxBorder.Rounded).LeftJustified();
                    AnsiConsole.Write(inputRule);
                    AnsiConsole.WriteLine();
                    AnsiConsole.WriteLine("Please enter start date and time: " + sessionState["startTime"] + "\n");
                }

                if (sessionState["endTime"] == "")
                {
                    string endTime = GetUserInput("End Time (Format: dd-MM-yyyy HH:mm:ss)", "Please enter end date and time");

                    // Exit if input is "0"
                    // Otherwise check if input is valid
                    if (endTime == "0")
                    {
                        ResetState();
                        break;
                    }
                    else if (!DateTime.TryParseExact(endTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        sessionState["endTime"] = "";
                        InvalidInputNotice();
                        continue;
                    }
                    else
                    {
                        isValidEndTime = true;
                        sessionState["endTime"] = endTime;
                    }
                }
            }

            var duration = CalculateDuration(sessionState["startTime"], sessionState["endTime"]);

            using (IDbConnection connection = new SQLiteConnection(_connectionString))
            {
                var sql = $"INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES ('{sessionState["startTime"]}', '{sessionState["endTime"]}', '{duration}')";

                connection.Execute(sql);
            }

            ResetState();
        }



        private double CalculateDuration(string startTime, string endTime) {
            DateTime startDateTime = DateTime.ParseExact(startTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(endTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (endDateTime - startDateTime).TotalHours;
        }

        // Templates
        private string GetUserInput(string header, string prompt)
        {
            var inputRule = new Spectre.Console.Rule(header).Border(BoxBorder.Rounded).LeftJustified();
            AnsiConsole.Write(inputRule);
            AnsiConsole.WriteLine();
            string input = AnsiConsole.Ask<string>($"{prompt}: ");
            AnsiConsole.WriteLine();

            return input;
        }
        private void AnyKeyPrompt()
        {
            var rule = new Spectre.Console.Rule().Border(BoxBorder.Double);
            AnsiConsole.Write(rule);
            AnsiConsole.Write("Press any key to continue.");
            Console.ReadKey();
        }

        private void InvalidInputNotice()
        {
            AnsiConsole.Write(new Panel(
                Align.Center(
                    new Markup("[red]Invalid date time format.\nPlease press any key to try again.[/]"),
                    VerticalAlignment.Middle
                    )
                ).Border(BoxBorder.Ascii).BorderStyle(Color.Red)
            );
            Console.ReadKey();
        }
        private void ResetState()
        {
            sessionState["startTime"] = "";
            sessionState["endTime"] = "";
        }
    }
}
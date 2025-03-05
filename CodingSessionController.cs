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
        Dictionary<string, string> sessionState = new Dictionary<string, string> { { "startTime", "" }, { "endTime", "" } };
        readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        public void ShowSessions()
        {
            AnsiConsole.Clear();

            var sessionData = GetAllSessions();

            if (sessionData.Count == 0)
            {
                Templates.GeneralNotice("No sessions are found...");
            }
            else
            {
                foreach (var sessions in sessionData)
                {
                    Console.WriteLine($"{sessions.Id} - Start: {sessions.StartTime} - End: {sessions.EndTime} - Duration: {sessions.Duration} hours");
                }
            }

            Templates.AnyKeyPrompt();
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
                    string startTime = Templates.GetUserInput("Start Time (Format: dd-MM-yyyy HH:mm:ss)", "Please enter start date and time");

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
                        Templates.InvalidInputNotice();
                        Templates.AnyKeyPrompt();
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
                    // Rewrite the first prompt and input if the second input is invalid
                    var inputRule = new Spectre.Console.Rule("Start Time (Format: dd-MM-yyyy HH:mm:ss)").Border(BoxBorder.Rounded).LeftJustified();
                    AnsiConsole.Write(inputRule);
                    AnsiConsole.WriteLine();
                    AnsiConsole.WriteLine("Please enter start date and time: " + sessionState["startTime"] + "\n");
                }

                // Handling end time input
                if (sessionState["endTime"] == "")
                {
                    string endTime = Templates.GetUserInput("End Time (Format: dd-MM-yyyy HH:mm:ss)", "Please enter end date and time");

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
                        Templates.InvalidInputNotice();
                        Templates.AnyKeyPrompt();
                        continue;
                    }
                    else
                    {
                        isValidEndTime = true;
                        sessionState["endTime"] = endTime;
                    }
                }
            }

            if (isValidStartTime && isValidEndTime)
            {
                var duration = CalculateDuration(sessionState["startTime"], sessionState["endTime"]);

                using IDbConnection connection = new SQLiteConnection(_connectionString);

                var sql = $"INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES ('{sessionState["startTime"]}', '{sessionState["endTime"]}', '{duration}')";

                connection.Execute(sql);
            }

            ResetState();
        }

        public void DeleteSession()
        {
            bool confirmExit = false;

            var sessionData = GetAllSessions();

            while (!confirmExit)
            {
                if (sessionData.Count == 0)
                {
                    break;
                }



                using IDbConnection connection = new SQLiteConnection(_connectionString);

            }
        }

        private List<CodingSession> GetAllSessions()
        {
            using IDbConnection connection = new SQLiteConnection(_connectionString);

            string query = "SELECT * FROM CodingSessions";

            return [.. connection.Query<CodingSession>(query)];
        }

        private double CalculateDuration(string startTime, string endTime)
        {
            DateTime startDateTime = DateTime.ParseExact(startTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(endTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (endDateTime - startDateTime).TotalHours;
        }

        private void ResetState()
        {
            sessionState["startTime"] = "";
            sessionState["endTime"] = "";
        }
    }
}
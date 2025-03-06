using System.Globalization;
using Spectre.Console;

namespace Code_Tracker
{
    public class UserInput
    {
        bool closeApp = false;
        string[] Options = ["Add Session", "Delete Session", "Update Session", "Show Sessions", "Exit"];

        // Used as a state for coding sessions when creating new ones
        Dictionary<string, string> sessionState = new Dictionary<string, string> { { "startTime", "" }, { "endTime", "" } };
        CodingSessionController controller = new();
        Validation validation = new();

        // Main Menu
        public void ShowMainMenu()
        {
            while (!closeApp)
            {
                AnsiConsole.Clear();

                var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

                AnsiConsole.Write(rule);

                var selectedOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please choose the options below:")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options.)[/]")
                        .AddChoices(Options));

                switch (selectedOption)
                {
                    case "Add Session":
                        AddSession();
                        break;
                    case "Delete Session":
                        RemoveSession();
                        break;
                    case "Update Session":
                        break;
                    case "Show Sessions":
                        ShowSessions();
                        break;
                    case "Exit":
                        AnsiConsole.Clear();
                        var panel = new Panel("Goodbye...")
                            .Header(new PanelHeader("Notice"))
                            .Border(BoxBorder.Double)
                            .Padding(new Padding(2, 2, 2, 2));
                        AnsiConsole.Write(panel);
                        closeApp = true;
                        break;
                    default:
                        AnsiConsole.Clear();
                        AnsiConsole.WriteLine("Invalid input");
                        AnsiConsole.Record();
                        break;
                }
            }
            ;
        }

        private void ShowSessions()
        {
            AnsiConsole.Clear();

            var sessionData = controller.GetSessions();

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

        private void AddSession()
        {
            AnsiConsole.Clear();

            var panel = new Panel(
                Align.Center(
                    new Markup($"[blue]CREATING SESSION[/]"),
                    VerticalAlignment.Middle
                    )
                ).Border(BoxBorder.Heavy).BorderStyle(Color.Blue);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            DateTimeInputHandler();

            controller.CreateSession(sessionState["startTime"], sessionState["endTime"]);

            ResetState();
        }

        private void RemoveSession()
        {
            bool confirmExit = false;

            var sessionData = controller.GetSessions();

            while (!confirmExit)
            {
                if (sessionData.Count == 0)
                {
                    break;
                }
            }
        }

        private void DateTimeInputHandler()
        {
            bool isValidStartTime = false;
            bool isValidEndTime = false;

            // Handles user input
            while (!isValidStartTime | !isValidEndTime)
            {
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
                    else if (!validation.ValidateDateTime(startTime))
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
                    var inputRule = new Rule("Start Time (Format: dd-MM-yyyy HH:mm:ss)").Border(BoxBorder.Rounded).LeftJustified();
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
                    else if (!validation.ValidateDateTime(endTime))
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
        }

        private void ResetState()
        {
            sessionState["startTime"] = "";
            sessionState["endTime"] = "";
        }
    }
}
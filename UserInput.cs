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

                AnsiConsole.Clear();

                switch (selectedOption)
                {
                    case "Add Session":
                        AddSession();
                        break;
                    case "Delete Session":
                        RemoveSession();
                        break;
                    case "Update Session":
                        EditSession();
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

            var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();

            var sessionData = controller.GetSessions();

            if (sessionData.Count == 0)
            {
                Templates.GeneralNotice("[blue]No sessions are found...[/]", BoxBorder.Rounded, Color.Blue);
            }
            else
            {
                var table = new Table();

                table.AddColumns(["ID", "Start", "End", "Duration (hours)"]).Centered().Title("Coding Sessions");

                int colorIncrement = 0;
                List<string> colorList = ["blue", "yellow"];

                foreach (var s in sessionData)
                {
                    table.AddRow(new Markup($"[{colorList[colorIncrement]}]{s.Id}[/]"), new Markup($"[{colorList[colorIncrement]}]{s.StartTime}[/]"), new Markup($"[{colorList[colorIncrement]}]{s.EndTime}[/]"), new Markup($"[{colorList[colorIncrement]}]{s.Duration}[/]"));

                    // switch between colors in colorList base on colorIncrement
                    colorIncrement++;
                    if (colorIncrement == colorList.Count) colorIncrement = 0;
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine();

            Templates.AnyKeyPrompt();
        }

        private void AddSession()
        {
            var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();

            DateTimeInputHandler();

            if (sessionState["startTime"] != "" || sessionState["endTime"] != "")
            {
                controller.CreateSession(sessionState["startTime"], sessionState["endTime"]);

                AnsiConsole.Clear();
                Templates.GeneralNotice("[green]Session added successfully[/]", BoxBorder.Rounded, Color.Green);
                Templates.AnyKeyPrompt();
            }

            ResetState();
        }

        private void RemoveSession()
        {
            bool confirmExit = false;

            var sessionData = controller.GetSessions();

            while (!confirmExit)
            {
                AnsiConsole.Clear();

                var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

                AnsiConsole.Write(rule);
                AnsiConsole.WriteLine();

                List<string> formattedSessionsList = [];

                if (sessionData.Count == 0)
                {
                    Templates.GeneralNotice("[blue]No sessions are found...[/]", BoxBorder.Rounded, Color.Blue);
                    Templates.AnyKeyPrompt();
                    break;
                }

                foreach (var s in sessionData)
                {
                    formattedSessionsList.Add($"ID: {s.Id}\t\tTimeframe: from {s.StartTime} to {s.EndTime}\t\tDuration: {s.Duration} hours");
                }

                var selectedSessions = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Please choose coding session(s) you want to remove. Choose none to cancel.")
                        .NotRequired() // Not required to have a favorite fruit
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more sessions.)[/]")
                        .InstructionsText(
                            "[grey](Press [blue]<space>[/] to toggle a coding session, " +
                            "[green]<enter>[/] to continue)[/]")
                        .AddChoices(
                            formattedSessionsList
                        )
                );

                if (selectedSessions.Count == 0)
                {
                    confirmExit = true;
                    break;
                }
                else
                {
                    foreach (var s in selectedSessions)
                    {
                        controller.DeleteSession(s.Substring(4, 1));
                    }
                }

                Templates.GeneralNotice("[green]Session(s) removed successfully[/]", BoxBorder.Rounded, Color.Green);

                Templates.AnyKeyPrompt();
            }
        }

        private void EditSession()
        {
            bool confirmExit = false;

            var sessionData = controller.GetSessions();

            while (!confirmExit)
            {
                AnsiConsole.Clear();

                var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

                AnsiConsole.Write(rule);
                AnsiConsole.WriteLine();

                List<string> formattedSessionsList = [];

                foreach (var s in sessionData)
                {
                    formattedSessionsList.Add($"ID: {s.Id}\tTimeframe: from {s.StartTime} to {s.EndTime}\tDuration: {s.Duration} hours");
                }

                // Add the cancel option at the first index to the options list
                formattedSessionsList.Insert(0, "Cancel");

                var selectedSession = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please choose a session to edit (Enter 0 to cancel):")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options.)[/]")
                        .AddChoices(formattedSessionsList)
                );

                if (selectedSession == "Cancel")
                {
                    confirmExit = true;
                    break;
                }

                var sessionId = selectedSession.Substring(4, 1);

                DateTimeInputHandler();

                if (sessionState["startTime"] != "" || sessionState["endTime"] != "")
                {
                    controller.UpdateSession(sessionId, sessionState["startTime"], sessionState["endTime"]);

                    AnsiConsole.Clear();
                    Templates.GeneralNotice("[green]Session updated successfully[/]", BoxBorder.Rounded, Color.Green);
                    Templates.AnyKeyPrompt();
                }
            }

            ResetState();
        }

        private void DateTimeInputHandler()
        {
            bool isValidStartTime = false;
            bool isValidEndTime = false;

            // Handles user input
            while (!isValidStartTime | !isValidEndTime)
            {
                AnsiConsole.Clear();

                var rule = new Rule("Code Tracker").Border(BoxBorder.Double);

                AnsiConsole.Write(rule);
                AnsiConsole.WriteLine();

                var panel = new Panel(
                Align.Center(
                    new Markup($"[blue]CREATING SESSION[/]"),
                    VerticalAlignment.Middle
                    )
                ).Border(BoxBorder.Heavy).BorderStyle(Color.Blue);

                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine();

                // var rule = new Rule("Enter 0 to cancel.").Border(BoxBorder.Double).LeftJustified();

                // AnsiConsole.Write(rule);
                // AnsiConsole.WriteLine();

                // Handling start time input
                if (sessionState["startTime"] == "")
                {
                    string startTime = Templates.GetUserInput("Start Time (Format: dd-MM-yyyy HH:mm:ss, Enter 0 to cancel.)", "Please enter start date and time");

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
                    string endTime = Templates.GetUserInput("End Time (Format: dd-MM-yyyy HH:mm:ss, Enter 0 to cancel.)", "Please enter end date and time");

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
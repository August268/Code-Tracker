using Spectre.Console;

namespace Code_Tracker
{
    public class UserInput
    {
        bool closeApp = false;
        string[] Options = ["Add Session", "Delete Session", "Update Session", "Show History", "Exit"];
        CodingSessionController controller = new();
        
        // Main Menu
        public void ShowMainMenu()
        {
            Console.Clear();

            while (!closeApp){

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
                        controller.CreateSession();
                        break;
                    case "Delete Session":
                        break;
                    case "Update Session":
                        break;
                    case "Show History":
                        controller.GetAllSessions();
                        break;
                    case "Exit":
                        Console.Clear();
                        var panel = new Panel("Goodbye...")
                            .Header(new PanelHeader("Notice"))
                            .Border(BoxBorder.Double)
                            .Padding(new Padding(2, 2, 2, 2));
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
            }
        
    }
}
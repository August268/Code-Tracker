using Spectre.Console;

namespace Code_Tracker
{
    public class Templates
    {
        public static string GetUserInput(string header, string prompt)
        {
            var inputRule = new Rule(header).Border(BoxBorder.Rounded).LeftJustified();
            AnsiConsole.Write(inputRule);
            AnsiConsole.WriteLine();
            string input = AnsiConsole.Ask<string>($"{prompt}: ");
            AnsiConsole.WriteLine();

            return input;
        }
        public static void AnyKeyPrompt()
        {
            var rule = new Rule().Border(BoxBorder.Double);
            AnsiConsole.Write(rule);
            AnsiConsole.Write("Press any key to continue.");
            Console.ReadKey();
        }

        public static void GeneralNotice(Markup notice, BoxBorder borderStyle, Style color)
        {
            AnsiConsole.Write(new Panel(
                Align.Center(
                    notice,
                    VerticalAlignment.Middle
                    )
                ).Border(borderStyle).BorderStyle(color)
            );
        }

        public static void InvalidInputNotice()
        {
            AnsiConsole.Write(new Panel(
                Align.Center(
                    new Markup("[red]Invalid input.\nPlease press any key to try again.[/]"),
                    VerticalAlignment.Middle
                    )
                ).Border(BoxBorder.Ascii).BorderStyle(Color.Red)
            );
        }
    }
}
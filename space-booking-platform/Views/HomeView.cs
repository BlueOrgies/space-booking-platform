using Spectre.Console;

namespace space_booking_platform.Views;

public class HomeView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Home[/]").RuleStyle("green"));

        var choices = new List<string> { "Log in", "Register", "Log out"};

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Welcome!")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));
        
        return choice switch
        {
            "Log in"             => "Login",
            "Register"          => "Register",
            "Log out"            => null
        };
    }
}
using Spectre.Console;

namespace space_booking_platform.Views;

public class HomeView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]✓ Build completed successfully[/]");

        var choices = new List<string> { "Log in", "Register", "Log out"};

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where do you want to go?")
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
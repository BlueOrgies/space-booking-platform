using Spectre.Console;

namespace space_booking_platform.Views;

public class LoginView(AppState state)
{
        /// <summary>
    /// Displays the view and handles user interaction.
    /// </summary>
    /// <returns>The name of the next view to display.</returns>
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Login[/]").RuleStyle("green"));
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Username:[/]")
                .PromptStyle("yellow"));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Password:[/]")
                .PromptStyle("yellow")
                .Secret());

        var userService = new UserService();
        var user = userService.Login(username, password);

        if (user is null)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red]Invalid username or password.[/]");

            var retry = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nWhat would you like to do?")
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices("Try again", "Register instead", "Quit"));

            return retry switch
            {
                "Try again" => "Login",
                "Register instead" => "Register",
                _ => null
            };
        }

        state.IsLoggedIn = true;
        state.CurrentUser = user.Username;
        state.IsOrganizer = user.IsOrganizer;
        state.CurrentUUID = user.UserId;
        state.CurrentUserWeight = user.Weight;

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold green]Welcome back, {user.Username}![/]");
        Thread.Sleep(1000);
        return "Home";
    }
}

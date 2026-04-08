using Spectre.Console;

namespace space_booking_platform.Views;

public class RegisterView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Register[/]").RuleStyle("green"));
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Choose a username:[/]")
                .PromptStyle("yellow")
                .Validate(u => u.Trim().Length >= 3
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Username must be at least 3 characters.[/]")));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Choose a password:[/]")
                .PromptStyle("yellow")
                .Secret()
                .Validate(p => p.Length >= 6
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Password must be at least 6 characters.[/]")));

        AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Confirm password:[/]")
                .PromptStyle("yellow")
                .Secret()
                .Validate(confirm => confirm == password
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Passwords do not match.[/]")));

        var weight = AnsiConsole.Prompt(
            new TextPrompt<int>("[green]Your weight (kg):[/]")
                .PromptStyle("yellow")
                .Validate(w => w > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Weight must be greater than 0.[/]")));

        var height = AnsiConsole.Prompt(
            new TextPrompt<int>("[green]Your height (cm):[/]")
                .PromptStyle("yellow")
                .Validate(h => h > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Height must be greater than 0.[/]")));

        var isOrganizer = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Register as an organizer?[/]")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("No", "Yes")) == "Yes";

        try
        {
            var userService = new UserService();
            var user = userService.Register(username.Trim(), password, weight, height, isOrganizer);

            state.isLoggedIn = true;
            state.currentUser = user.Username;
            state.isOrganizer = user.IsOrganizer;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold green]Account created! Welcome, {user.Username}![/]");
            Thread.Sleep(1000);
            return "Home";
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");

            var retry = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nWhat would you like to do?")
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices("Try again", "Login instead", "Quit"));

            return retry switch
            {
                "Try again" => "Register",
                "Login instead" => "Login",
                _ => null
            };
        }
    }
}

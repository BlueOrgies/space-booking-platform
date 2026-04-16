using Spectre.Console;

namespace space_booking_platform.Views;

public class HomeView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Home[/]").RuleStyle("green"));
        
        var choices = new List<string> { "Login", "Register", "Quit" };
        if (state.isLoggedIn)
        {
            choices = new List<string> { "BrowseListings", "SearchListings", "ProfileView" };
            if (state.isOrganizer)
            {
                choices.Add("OrganizerView");
            }
            choices.Add("Logout");
        }
        

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Welcome!")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));
        
        return choice switch
        {
            "Login"             => "Login",
            "Register"          => "Register",
            "BrowseListings"     => "BrowseListings",
            "SearchListings"     => "SearchListings",
            "ProfileView"       => "ProfileView",
            "OrganizerView"     => "OrganizerView",
            "Logout"            => "Logout",
            _                   => null // quit
        };
    }
}
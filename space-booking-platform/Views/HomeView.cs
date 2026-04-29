using Spectre.Console;

namespace space_booking_platform.Views;

public class HomeView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Home[/]").RuleStyle("green"));
        
        var choices = new List<string> { "Login", "Register", "Quit" };
        if (state.IsLoggedIn)
        {
            choices = new List<string> { "Browse Listings", "Search Listings", "View Profile" };
            if (state.IsOrganizer)
            {
                choices.Add("Organizer Dashboard");
            }
            choices.Add("Log out");
        }
        

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Welcome!")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));
        
        return choice switch
        {
            "Login"                => "Login",
            "Register"             => "Register",
            "Browse Listings"      => "BrowseListings",
            "Search Listings"      => "SearchListings",
            "View Profile"         => "ProfileView",
            "Organizer Dashboard"  => "OrganizerView",
            "Log out"               => "Logout",
            _                   => null // quit
        };
    }
}
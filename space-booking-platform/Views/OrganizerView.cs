using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        
        ListingService ls = new ListingService(state);
        AnsiConsole.MarkupLine($"[bold green]=== {state.currentUser}s profile. [/]===");

        AnsiConsole.MarkupLine("\n[green]My listings[/]");
        ls.ShowOverview(ls.ShowUserListings());
        AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        //TODO: Fix this when reviews is made 

        var choices = new List<string>
            { "View my listings", "View my reviews", "Go back to main menu", "Quit" };

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));

        return choice switch
        {
            "View my listings" => "MyListings",
            "View my reviews" => "MyReviews",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
    }
}
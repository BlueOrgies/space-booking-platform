using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        //TODO: Add name of user and average rating 
        //if (state.isOrganizer)
        {
            listingService ls = new listingService(state);
            AnsiConsole.MarkupLine("[bold green]=== *Users* profile. [/]===" +
                                   "\nAverage rating: *Average rating from db*");

            AnsiConsole.MarkupLine("\n[green]My listings[/]");
            ls.ShowOverview(ls.ShowUserListings());
            AnsiConsole.MarkupLine("\n[green]My bookings[/]");
            ls.ShowOverview(ls.ShowUserBookings());
            AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        }

        var choices = new List<string>
            { "View my listings", "View my bookings", "View my reviews", "Go back to main menu", "Quit" };

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));

        return choice switch
        {
            "View my listings" => "MyListings",
            "View my bookings" => "MyBookings",
            "View my reviews" => "MyReviews",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
        //TODO: Add these views? 
    }
}
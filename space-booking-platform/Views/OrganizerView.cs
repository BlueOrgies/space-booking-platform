using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerView
{
    public static void ViewProfileAsOrganizer()
    {
        //TODO: Add name of user and average rating 
        AnsiConsole.MarkupLine("[bold green]=== *Users* profile. [/]===" +
                               "\nAverage rating: *Average rating from db*");
        
        AnsiConsole.MarkupLine("\n[green]My listings[/]");
        ListingService.ShowOverview(ListingService.ShowListings(1));
        AnsiConsole.MarkupLine("\n[green]My bookings[/]");
        ListingService.ShowOverview(ListingService.ShowBookings(1));
        AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        
        var prompt = new SelectionPrompt<string>()
            .Title("[bold]What would you like to do?:[/]")
            .WrapAround()
            .AddChoices("Show my listings", "Show my bookings", "Show my reviews", "Go back to main menu", "Exit");
        string edit = AnsiConsole.Prompt(prompt);
        
        // TODO: Make this change view when it is up and running 
    }

}
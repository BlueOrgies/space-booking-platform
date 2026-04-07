using System.Data.SQLite;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]My listings[/]");
        
        ListingService.ShowMyListings();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhere would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Go back to profile",
                    "Go back to main menu", "Quit"));
        if (state.isOrganizer)
        {
            return choice switch
            {
                "Go back to profile" => "OrganizerView",
                "Go back to main menu" => "HomeView",
                _ => null // Quit
            };
        }
        return choice switch
        {
            "Go back to profile" => "ProfileView",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
    }
}
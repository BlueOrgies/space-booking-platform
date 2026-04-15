using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class ProfileView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
            
        ListingService ls = new ListingService(state);
        AnsiConsole.MarkupLine($"[bold green]=== {state.currentUser}s profile. [/]===");

        AnsiConsole.MarkupLine("\n[green]My Bookings[/]");
        ls.ShowOverview(ls.ShowUserBookings());

        var choices = new List<string>
            { "View my bookings", "Go back to main menu", "Quit" };

        var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Where would you like to go?")
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices(choices));

            return choice switch
        {
            "View my bookings" => "MyBookings",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
}
}
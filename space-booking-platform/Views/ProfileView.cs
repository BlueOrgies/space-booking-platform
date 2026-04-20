using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class ProfileView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();

        BookingService bs = new BookingService();
        var choices = new List<string> { "Go back to main menu", "Quit" };
        AnsiConsole.MarkupLine($"[bold green]=== {state.CurrentUser}s profile. [/]===");

        AnsiConsole.MarkupLine("\n[green]My Bookings[/]");
        var table = new Table()
            .SimpleBorder()
            .BorderColor(Color.Green);

        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());

        List<Booking?> bookings = bs.GetBookings(state.CurrentUUID);
        switch (bookings.Count)
        {
            case > 5:
            {
                for (int i = 0; i < 5; i++)
                {
                    Booking? booking = bookings[i];
                    table.AddRow(booking.Category.ToString(), booking.Title, booking.Origin, 
                        booking.Destination, booking.Date.ToString("o"), booking.BookingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Add("View my bookings");
                break;
            }
            case > 0:
            {
                foreach (var booking in bookings)
                {
                    table.AddRow(booking.Category.ToString(), booking.Title, booking.Origin, 
                        booking.Destination, booking.Date.ToString("o"), booking.BookingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Add("View my bookings");
                break;
            }
            case 0:
                AnsiConsole.MarkupLine("No bookings found");
                break;
        }
        Console.WriteLine();

        var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Where would you like to go?")
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices(choices));

            return choice switch
        {
            "View my bookings" => "MyBookings",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
}
}
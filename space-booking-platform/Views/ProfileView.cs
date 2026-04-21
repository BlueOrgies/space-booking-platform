using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class ProfileView(AppState state)
{
    private const int Limit = 5;
    public string? Display()
    {
        AnsiConsole.Clear();

        BookingService bs = new BookingService();
        var choices = new List<string> { "Main menu" };
        AnsiConsole.Write(new Rule($"[bold green]{state.CurrentUser}s profile[/]").RuleStyle("green"));

        AnsiConsole.MarkupLine("\n[green]My bookings[/]");
        var table = new Table()
            .SimpleBorder()
            .BorderColor(Color.Green);

        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());
        
        List<Booking?> bookings = bs.GetLimitedBookings(state.CurrentUUID, Limit);
        if (bookings.Count == 0)
        {
            AnsiConsole.MarkupLine("No bookings found");
        }
        else
        {
            foreach (Booking? booking in bookings)
            {
                    table.AddRow(booking.Category.ToString(), booking.Title, booking.Origin, 
                        booking.Destination, booking.Date.ToString("o"), booking.BookingStatus.ToString());
            }
            AnsiConsole.Write(table);
            choices.Add("View my bookings");
        }
        
        
        Console.WriteLine();

        var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices(choices));

            return choice switch
        {
            "View my bookings" => "MyBookings",
            "Main menu" => "Home",
            _ => null // Quit
        };
}
}
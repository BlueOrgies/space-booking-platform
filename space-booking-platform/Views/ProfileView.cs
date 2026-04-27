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
        UserService us =  new UserService();
        User? user = us.GetById(state.CurrentUUID);
        
        var choices = new List<string> { "Main menu" };
        AnsiConsole.Write(new Rule($"[bold green]{user.Username}s profile[/]").RuleStyle("green"));
        
        AnsiConsole.MarkupLine("\n[green]My info[/]");
        var grid = new Grid();
  
        grid.AddColumn(new GridColumn { Alignment = Justify.Left });
        grid.AddColumn(new GridColumn { Alignment = Justify.Left });
  
        grid.AddRow("User created at:", $"[green]{user.CreatedAt}[/]");
        grid.AddRow("Weight:", $"[green]{user.Height} cm[/]");
        grid.AddRow("Height:", $"[green]{user.Weight} kg[/]");
  
        AnsiConsole.Write(grid);

        AnsiConsole.MarkupLine("\n[green]My bookings[/]");
        var table = new Table()
            .RoundedBorder()
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
            choices.Insert(0, "View my bookings");
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
namespace space_booking_platform.Views;

public class MyBookingsView(AppState state)
{
    public string? Display()
    {
        BookingService bookingService = new BookingService();
        List<Booking?> allBookings = bookingService.GetBookings(state.currentUUID);

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]My Bookings[/]");
            if (allBookings.Count == 0)
            {
                AnsiConsole.MarkupLine("You have no bookings.");
                AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("Go back to profile"));
                return "ProfileView";
            }
        }
    }
}
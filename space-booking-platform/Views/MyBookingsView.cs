using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyBookingsView(AppState state)
{
    private const int PageSize = 5;
    public string? Display()
    {
        int currentPage = 0;
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
            
            int startIndex = currentPage * PageSize;
            int endIndex = Math.Min(startIndex + PageSize, allBookings.Count);
            int totalPages = (int)Math.Ceiling((double)allBookings.Count / PageSize);

            var rows = new Dictionary<string, string>();
            for (int i = startIndex; i < endIndex; i++)
            {
                Booking? booking = allBookings[i];
                if (booking == null) continue;
                
                string text = $"{booking.Category} | {booking.Title} | {booking.Origin} -> {booking.Destination} | " +
                              $"{booking.Date:dd.MM.yyyy} | {booking.BookingStatus}";
                rows.Add(text, booking.ListingId.ToString());
            }
            
            var prompt = new SelectionPrompt<string>()
                .Title($"\nShowing page {currentPage + 1} of {totalPages}. Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow));
        }
    }
}
using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyBookingsView(AppState state)
{
    private const int PageSize = 5;
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]My Bookings[/]").RuleStyle("green"));
        
        BookingService bookingService = new BookingService();
        
        Dictionary<string, string> rows = new Dictionary<string, string>();
        List<Booking?> bookings = bookingService.GetLimitedBookings(state.CurrentUUID, PageSize, state.Offset);
        
        if (bookings.Count > 0) 
        {
            foreach (var booking in bookings)
            {
                string text = $"{booking.Category} | {booking.Title} | {booking.Origin} -> {booking.Destination} | " +
                              $"Booked at: {booking.Date:dd.MM.yyyy} | {booking.BookingStatus}";
                rows.Add(text, booking.ListingId.ToString());
                
            }
        }
        else
        {
            AnsiConsole.MarkupLine("You have no bookings.");
        }
        
        var choices = new List<string> { "Go back to profile", "Go back to main menu" };

        if (bookings.Count == 5) 
        {
            choices.Insert(0, "Next page");
        }

        if (state.Offset > 0)
        {
            choices.Insert(0, "Previous page");
        }
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoiceGroup("", rows.Keys.ToArray())
                .AddChoiceGroup("", choices));
        
            if (rows.TryGetValue(choice, out string? listingId))
            {
                state.CurrentListingID = int.Parse(listingId);
                return "Listing";
            }
            
        switch (choice) 
        { 
            case "Next page": 
                state.Offset += PageSize; 
                return "MyBookings";
                break;
            case "Previous page": 
                state.Offset -= PageSize; 
                return "MyBookings";
                break;
            case "Go back to profile": 
                return "ProfileView";
            case "Go back to main menu": 
                return "Home";
            case "Quit": 
                return null;
            default: 
                throw new Exception("Invalid choice");
        }
    }
}
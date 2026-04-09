using System.Net.Http.Headers;
using space_booking_platform.Views;

namespace space_booking_platform;

public class ViewHandler
{
    private readonly AppState _state = new();

    public void Run(string startView)
    {
        string? current = startView;
        while (current != null)
            current = Dispatch(current);
    }
    
    private string? Dispatch(string viewName) => viewName switch
    {
        "Home"              => new HomeView(_state).Display(),
        "Register"          => NotImplemented("RegisterView"),
        "Login"             => NotImplemented("Login"),
        "BrowseListing"     => NotImplemented("BrowseListingView"),
        "SearchListing"     => NotImplemented("SearchView"),
        "Booking"           => NotImplemented("BookingView"),
        "ProfileView"       => NotImplemented("ProfileView"),
        "MyBookings"        => NotImplemented("MyBookingsView"),
        "MyListings"        => new MyListingsView(_state).Display(),
        "OrganizerView"     => new OrganizerView(_state).Display(),
        "CreateListing"     => new CreateListingView(_state).Display(),
        "EditListing"       => new EditListingView(_state).Display(),
        "Review"            => NotImplemented("ReviewView"),
        _ => throw new ArgumentException($"Unknown view: {viewName}")
    };

    private string? NotImplemented(string viewName)
    {
        Console.WriteLine($"{viewName} is not available yet.");
        Console.WriteLine($"Press any key to go back: ");
        Console.ReadKey();
        return "Home";
    }
}
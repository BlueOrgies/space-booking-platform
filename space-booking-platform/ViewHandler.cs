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
        "Register"          => new RegisterView(_state).Display(),
        "Login"             => new LoginView(_state).Display(),
        "BrowseListings"     => new BrowseListingsView(_state).Display(),
        "SearchListings"     => new SearchListingsView(_state).Display(),
        "Booking"           => NotImplemented("BookingView"),
        "ProfileView"       => new ProfileView(_state).Display(),
        "MyBookings"        => NotImplemented("MyBookingsView"),
        "MyListings"        => new MyListingsView(_state).Display(),
        "Listing"           => new ListingView(_state).Display(),
        "OrganizerView"     => new OrganizerView(_state).Display(),
        "CreateListing"     => new CreateListingView(_state).Display(),
        "EditListing"       => new EditListingView(_state).Display(),
        "Review"            => NotImplemented("ReviewView"),
        "Logout"            => Logout(),
        _ => throw new ArgumentException($"Unknown view: {viewName}")
    };

    private string? NotImplemented(string viewName)
    {
        Console.WriteLine($"{viewName} is not available yet.");
        Console.WriteLine($"Press any key to go back: ");
        Console.ReadKey();
        return "Home";
    }

    private string? Logout()
    {
        _state.isLoggedIn = false;
        _state.currentUser = null;
        return "Home";
    }
}
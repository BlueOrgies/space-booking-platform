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
        "Home"              => NotImplemented("HomeView"),
        "Register"          => NotImplemented("RegisterView"),
        "Login"             => NotImplemented("Login"),
        "BrowseListing"     => NotImplemented("BrowseListingView"),
        "SearchListing"     => NotImplemented("SearchView"),
        "Booking"           => NotImplemented("BookingView"),
        "ProfileView"       => NotImplemented("ProfileView"),
        "MyBookings"        => NotImplemented("MyBookingsView"),
        "Organizer"         => NotImplemented("OrganizerView"),
        "CreateListing"     => NotImplemented("CreateListingView"),
        "EditListing"       => NotImplemented("EditListingView"),
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
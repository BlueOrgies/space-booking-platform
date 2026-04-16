namespace space_booking_platform;

public class AppState
{
    public bool isLoggedIn { get; set; }
    public bool isOrganizer { get; set; }
    public string? currentUser { get; set; }
    public int currentUUID { get; set; }
    public int currentListingID { get; set; }
    public int currentBookingID { get; set; }
    public int currentReviewID { get; set; }
}
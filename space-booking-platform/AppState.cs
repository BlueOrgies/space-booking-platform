namespace space_booking_platform;

public class AppState
{
    public bool IsLoggedIn { get; set; }
    public bool IsOrganizer { get; set; }
    public string? CurrentUser { get; set; }
    public int CurrentUUID { get; set; }
    public int CurrentListingID { get; set; }
    public int CurrentBookingID { get; set; }
    public int CurrentReviewID { get; set; }
    public int CurrentUserWeight { get; set; }
    public int CurrentPage {get; set;}
    public int Offset { get; set; }
}
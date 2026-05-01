namespace space_booking_platform.Models;

public class Activity : Listings
{
    public int MinAge { get; set; }
    public string Location { get; set; } = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="Activity"/> class.
    /// </summary>
    public Activity()
    {
        Category = ListingCategory.Activity;
    }
}
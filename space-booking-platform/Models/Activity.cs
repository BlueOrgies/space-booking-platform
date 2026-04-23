namespace space_booking_platform.Models;

public class Activity : Listings
{
    public int MinAge { get; set; }
    public string Location { get; set; } = string.Empty;
    public Activity()
    {
        Category = ListingCategory.Activity;
    }
}
namespace space_booking_platform.Models;

public class Activity : Listings
{
    public int MinAge { get; set; }
    public Activity()
    {
        Category = ListingCategory.Activity;
    }
}
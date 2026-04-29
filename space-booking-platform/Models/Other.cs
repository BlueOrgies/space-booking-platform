namespace space_booking_platform.Models;

public class Other : Listings
{
    public string AdditionalInfo { get; set; } = string.Empty;
    public Other()
    {
        Category = ListingCategory.Other;
    }
}
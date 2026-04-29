namespace space_booking_platform.Models;

public class Accommodation : Listings
{
    public bool PetsAllowed { get; set; }
    public string Location { get; set; } = string.Empty;
    public Accommodation()
    {
        Category = ListingCategory.Accommodation;
    }
}
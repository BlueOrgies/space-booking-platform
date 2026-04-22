namespace space_booking_platform.Models;

public class Accommodation : Listings
{
    public bool PetsAllowed { get; set; }
    public Accommodation()
    {
        Category = ListingCategory.Accommodation;
    }
}
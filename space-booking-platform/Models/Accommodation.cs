namespace space_booking_platform.Models;

public class Accommodation : Listings
{
    public bool PetsAllowed { get; set; }
    public string Location { get; set; } = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="Accommodation"/> class.
    /// </summary>
    public Accommodation()
    {
        Category = ListingCategory.Accommodation;
    }
}
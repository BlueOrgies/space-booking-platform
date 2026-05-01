namespace space_booking_platform.Models;

public class Other : Listings
{
    public string AdditionalInfo { get; set; } = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="Other"/> class.
    /// </summary>
    public Other()
    {
        Category = ListingCategory.Other;
    }
}
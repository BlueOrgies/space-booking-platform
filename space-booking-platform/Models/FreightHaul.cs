namespace space_booking_platform.Models;

public class FreightHaul : Listings
{
    public bool HazardousMaterialsAllowed { get; set; }
    public string TransportMethod { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="FreightHaul"/> class.
    /// </summary>
    public FreightHaul()
    {
        Category = ListingCategory.FreightHaul;
    }
}
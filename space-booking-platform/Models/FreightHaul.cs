namespace space_booking_platform.Models;

public class FreightHaul : Listings
{
    public bool HazardousMaterialsAllowed { get; set; }
    public FreightHaul()
    {
        Category = ListingCategory.FreightHaul;
    }
}
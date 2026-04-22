namespace space_booking_platform.Models;

public class PassengerTransportation : Listings
{
    public bool LuggageIncluded { get; set; }
    public PassengerTransportation()
    {
        Category = ListingCategory.PassengerTransportation;
    }
}
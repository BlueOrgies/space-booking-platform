namespace space_booking_platform.Models;

public class PassengerTransportation : Listings
{
    public bool LuggageIncluded { get; set; }
    public string TransportMethod { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="PassengerTransportation"/> class.
    /// </summary>
    public PassengerTransportation()
    {
        Category = ListingCategory.PassengerTransportation;
    }
}
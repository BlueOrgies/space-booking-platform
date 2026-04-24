using System.ComponentModel;

namespace space_booking_platform.Models;

public enum ListingCategory
{
    [Description("Passenger transportation")]
    PassengerTransportation, 
    [Description("Accomodation")]
    Accommodation,
    [Description("Freight haul")]
    FreightHaul, 
    [Description("Activity")]
    Activity,
    [Description("Other")]
    Other
}

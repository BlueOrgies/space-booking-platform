using System.ComponentModel;

namespace space_booking_platform.Models;

public enum ListingCapacityUnit
{
    [Description("Beds")]
    Beds,
    [Description("Seats")]
    Seats,
    [Description("Max weight")]
    MaxWeight
}
using System.ComponentModel;

namespace space_booking_platform.Models;

public enum ListingPriceUnit
{
    [Description("Euros")]
    Euros,
    [Description("Euros/kilo")]
    EurosPerKg,
}
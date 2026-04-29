using System.ComponentModel;

namespace space_booking_platform.Models;

public enum ListingStatus
{
    [Description("Upcoming")]
    Upcoming,
    [Description("Past")]
    Past,
    [Description("Cancelled")]
    Cancelled,
    [Description("Sold out")]
    SoldOut
}
using System.ComponentModel;

namespace space_booking_platform.Models;

public class Listings
{
    public int ListingId { get; set; }
    public int UUID { get; set; }
    public ListingCategory Category { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public int Duration { get; set; }

    public string DurationType { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public ListingCapacityUnit CapacityUnit { get; set; }

    public decimal Price { get; set; }

    public ListingPriceUnit PriceUnit { get; set; }

    public DateTime CreatedAt { get; set; }

    public ListingStatus ListingStatus { get; set; }

}
public static class MyEnumExtensions
{
    public static string ToDescriptionString(this Enum val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
} 
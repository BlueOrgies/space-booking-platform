namespace space_booking_platform.Models;

public class Booking
{
    public int BookingId  { get; set; }
    
    public int UUID { get; set; }
    
    public int ListingId { get; set; }
    
    public ListingStatus BookingStatus { get; set; }
    
    public ListingCategory Category { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string TransportMethod { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

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
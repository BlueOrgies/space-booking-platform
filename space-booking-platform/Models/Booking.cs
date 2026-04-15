namespace space_booking_platform.Models;

public class Booking
{
    public int BookingId  { get; set; }
    
    public int UUID { get; set; }
    
    public int ListingId { get; set; }
    
    public ListingStatus BookingStatus { get; set; }
    
    public ListingCategory Category { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }

    public string TransportMethod { get; set; }

    public string Origin { get; set; }

    public string Destination { get; set; }

    public DateTime Date { get; set; }

    public int Duration { get; set; }

    public string DurationType { get; set; }

    public int Capacity { get; set; }

    public ListingCapacityUnit CapacityUnit { get; set; }

    public decimal Price { get; set; }

    public ListingPriceUnit PriceUnit { get; set; }

    public DateTime CreatedAt { get; set; }

    public ListingStatus ListingStatus { get; set; }
}
namespace space_booking_platform.Models;

public class Reviews
{
    public int UUID { get; set; }
    public int BookingID { get; set; }
    public int Rating  { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
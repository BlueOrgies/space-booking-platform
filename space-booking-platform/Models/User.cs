namespace space_booking_platform.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Weight { get; set; }
    public int Height { get; set; }
    public bool IsOrganizer { get; set; }
    public DateTime CreatedAt { get; set; }
}

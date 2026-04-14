using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform.Services;

public class ReviewService
{
    public Review GetReview(int id)
    {
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand("SELECT * FROM reviews WHERE reviewID = @id", myConn);
        command.Parameters.AddWithValue("@id", id);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        return MapReview(reader);
    }
    
    private static Review MapReview(SQLiteDataReader reader) => new Review
    {
        ReviewId = Convert.ToInt32(reader["reviewID"]),
        UUID = Convert.ToInt32(reader["UUID"]),
        BookingID = Convert.ToInt32(reader["bookingID"]),
        Rating = Convert.ToInt32(reader["rating"]),
        Comment = reader["weight"].ToString()!,
        CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!)
    };
}
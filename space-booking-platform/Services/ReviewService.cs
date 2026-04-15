using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform.Services;

public class ReviewService
{
    public Review? GetReview(int id)
    {
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand("SELECT * FROM reviews JOIN bookings ON bookings.bookingID = reviews.bookingID " +
                                                        "JOIN listings ON listings.listingID = bookings.listingID " +
                                                        "WHERE reviewID = @id", myConn);
        command.Parameters.AddWithValue("@id", id);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        
        if (!reader.Read())
            return null;
        
        return MapReview(reader);
    }
    
    private static Review MapReview(SQLiteDataReader reader) => new Review
    {
        ReviewId = Convert.ToInt32(reader["reviewID"]),
        UUID = Convert.ToInt32(reader["UUID"]),
        BookingID = Convert.ToInt32(reader["bookingID"]),
        Rating = Convert.ToInt32(reader["rating"]),
        Comment = reader["comment"].ToString()!,
        CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!),
        Type = reader["type"].ToString()!,
        Title = reader["title"].ToString()!
    };
}
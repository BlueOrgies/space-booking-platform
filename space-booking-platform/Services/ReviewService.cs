using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform.Services;

public class ReviewService
{
    public Review? GetReview(int reviewId)
    {
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand(
            "SELECT * FROM reviews JOIN bookings ON bookings.bookingID = reviews.bookingID " +
            "JOIN listings ON listings.listingID = bookings.listingID " +
            "WHERE reviewID = @id", myConn);
        command.Parameters.AddWithValue("@id", reviewId);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        
        if (!reader.Read())
            return null;
        
        return MapReview(reader);
    }
    
    public List<Review?> GetReviews(int UUID)
    {
        List<Review?> reviews = new List<Review?>();
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand("SELECT * FROM reviews JOIN bookings ON bookings.bookingID = reviews.bookingID " +
                                                        "JOIN listings ON listings.listingID = bookings.listingID " +
                                                        "WHERE listings.UUID = @id", myConn);
        command.Parameters.AddWithValue("@id", UUID);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        
        if (!reader.Read())
            return reviews;

        while (reader.Read())
        {
            Review review = MapReview(reader);
            reviews.Add(review);
        }
        return reviews;
    }

    public void CreateReview(int uuid, int bookingId, int rating, string comment)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "INSERT INTO reviews (UUID, bookingID, rating, comment, createdAt) " +
            "VALUES (@uuid, @bookingId, @rating, @comment, @createdAt)", conn);
        
        cmd.Parameters.AddWithValue("@uuid", uuid);
        cmd.Parameters.AddWithValue("@bookingId", bookingId);
        cmd.Parameters.AddWithValue("@rating", rating);
        cmd.Parameters.AddWithValue("@comment", comment);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
        
        cmd.ExecuteNonQuery();
    }

    public bool HasReview(int bookingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM reviews WHERE bookingID = @bookingId", conn);

        cmd.Parameters.AddWithValue("@bookingId", bookingId);
        return (long)cmd.ExecuteScalar()! > 0;
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
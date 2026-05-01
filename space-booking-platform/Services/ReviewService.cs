using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform.Services;

public class ReviewService
{
    /// <summary>
    /// Retrieves a specific review by its identifier.
    /// </summary>
    /// <param name="reviewId">The identifier of the review.</param>
    /// <returns>The review if found; otherwise, null.</returns>
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
    
    /// <summary>
    /// Retrieves all reviews for a specific user (as an organizer).
    /// </summary>
    /// <param name="UUID">The unique identifier of the user.</param>
    /// <returns>A list of reviews.</returns>
    public List<Review?> GetReviews(int UUID)
    {
        List<Review?> reviews = new List<Review?>();
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand("SELECT * FROM reviews JOIN bookings ON bookings.bookingID = reviews.bookingID " +
                                                        "JOIN listings ON listings.listingID = bookings.listingID " +
                                                        "WHERE listings.UUID = @id", myConn);
        command.Parameters.AddWithValue("@id", UUID);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            Review review = MapReview(reader);
            reviews.Add(review);
        }
        return reviews;
    }
    
    /// <summary>
    /// Retrieves a limited number of reviews for a specific user (as an organizer).
    /// </summary>
    /// <param name="UUID">The unique identifier of the user.</param>
    /// <param name="limit">The maximum number of reviews to return.</param>
    /// <returns>A list of reviews.</returns>
    public List<Review?> GetLimitedReviews(int UUID, int limit)
    {
        List<Review?> reviews = new List<Review?>();
        SQLiteConnection myConn = Database.ConnectToDb();
        
        using SQLiteCommand command = new SQLiteCommand("SELECT * FROM reviews JOIN bookings ON bookings.bookingID = reviews.bookingID " +
                                                        "JOIN listings ON listings.listingID = bookings.listingID " +
                                                        "WHERE listings.UUID = @id LIMIT @limit", myConn);
        command.Parameters.AddWithValue("@id", UUID);
        command.Parameters.AddWithValue("@limit", limit);
        
        using SQLiteDataReader reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            Review review = MapReview(reader);
            reviews.Add(review);
        }
        return reviews;
    }

    /// <summary>
    /// Creates a new review for a booking.
    /// </summary>
    /// <param name="uuid">The unique identifier of the user leaving the review.</param>
    /// <param name="bookingId">The identifier of the booking being reviewed.</param>
    /// <param name="rating">The rating given (e.g., 1-5).</param>
    /// <param name="comment">The review comment.</param>
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

    /// <summary>
    /// Checks if a review already exists for a specific booking.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <returns>True if a review exists; otherwise, false.</returns>
    public bool HasReview(int bookingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM reviews WHERE bookingID = @bookingId", conn);

        cmd.Parameters.AddWithValue("@bookingId", bookingId);
        return (long)cmd.ExecuteScalar()! > 0;
    }
    
    /// <summary>
    /// Calculates the average rating for a user's listings.
    /// </summary>
    /// <param name="currentUserId">The unique identifier of the user.</param>
    /// <returns>The average rating, or 0.0 if no reviews exist.</returns>
    public double GetAverageRating(int currentUserId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand("SELECT AVG(rating) FROM reviews " +
                                                    "JOIN bookings ON bookings.bookingID = reviews.bookingID " +
                                                    "JOIN listings ON listings.listingID = bookings.listingID " +
                                                    "WHERE listings.UUID = @uuid", conn);
        cmd.Parameters.AddWithValue("@uuid", currentUserId);
        var rating = cmd.ExecuteScalar();
        return rating == DBNull.Value ? 0.0 : Convert.ToDouble(rating);
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
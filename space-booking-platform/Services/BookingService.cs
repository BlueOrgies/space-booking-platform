using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform.Services;

public class BookingService
{
    public bool HasBooked(int uuid, int listingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM bookings WHERE UUID = @uuid AND listingID = @listingId", conn);
        cmd.Parameters.AddWithValue("@uuid", uuid);
        cmd.Parameters.AddWithValue("@listingId", listingId);
        return (long)cmd.ExecuteScalar()! > 0;
    }

    public void CreateBooking(int uuid, int listingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "INSERT INTO bookings (UUID, listingID, bookingStatus) VALUES (@uuid, @listingId, 'Confirmed')", conn);
        cmd.Parameters.AddWithValue("@uuid", uuid);
        cmd.Parameters.AddWithValue("@listingId", listingId);
        cmd.ExecuteNonQuery();
    }

    public List<Booking?> GetBookings(int id)
    {
        List<Booking?> bookings = new List<Booking?>();
        SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand command = new SQLiteCommand(
            "SELECT * FROM bookings JOIN main.listings ON listings.listingID = bookings.bookingID " +
            "WHERE bookings.UUID = @id", myConn);
        command.Parameters.AddWithValue("@id", id);

        using SQLiteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            return bookings;

        while (reader.Read())
        {
            Booking booking = MapBooking(reader);
            bookings.Add(booking);
        }

        return bookings;
    }
    
    private static Booking MapBooking(SQLiteDataReader reader) => new Booking
    {
        BookingId = Convert.ToInt32(reader["bookingID"]),
        UUID = Convert.ToInt32(reader["UUID"]),
        ListingId = Convert.ToInt32(reader["listingID"]),
        Category = ListingService.ParseListingCategory(reader),
        Title = reader["title"].ToString()!,
        Description = reader["description"].ToString()!,
        TransportMethod = reader["transportMethod"].ToString()!,
        Origin = reader["origin"].ToString()!,
        Destination = reader["destination"].ToString()!,
        Date = DateTime.Parse(reader["createdAt"].ToString()!), 
        Duration = Convert.ToInt32(reader["UUID"]),
        DurationType = reader["durationType"].ToString()!,
        Capacity = Convert.ToInt32(reader["UUID"]),
        CapacityUnit = ListingService.ParseListingCapacityUnit(reader),
        Price = Convert.ToInt32(reader["UUID"]),
        PriceUnit = ListingService.ParseListingPriceUnit(reader),
        ListingStatus = ListingService.ParseListingStatus(reader)
    };
    
}
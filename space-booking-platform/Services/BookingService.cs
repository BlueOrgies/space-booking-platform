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

    public int GetBookingCount(int listingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM bookings WHERE listingID = @listingId", conn);
        cmd.Parameters.AddWithValue("@listingId", listingId);
        return Convert.ToInt32((long)cmd.ExecuteScalar()!);
    }

    public int GetBookedWeight(int listingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT COALESCE(SUM(u.weight), 0) FROM bookings b JOIN users u ON u.UUID = b.UUID WHERE b.listingID = @listingId", conn);
        cmd.Parameters.AddWithValue("@listingId", listingId);
        return Convert.ToInt32(cmd.ExecuteScalar()!);
    }

    public void CreateBooking(int uuid, int listingId)
    {
        using SQLiteConnection conn = Database.ConnectToDb();

        using (SQLiteCommand checkCmd = new SQLiteCommand(
            "SELECT UUID, capacity FROM listings WHERE listingID = @listingId", conn))
        {
            checkCmd.Parameters.AddWithValue("@listingId", listingId);
            using var reader = checkCmd.ExecuteReader();
            if (reader.Read())
            {
                if (Convert.ToInt32(reader["UUID"]) == uuid)
                    throw new InvalidOperationException("You cannot book your own listing.");
            }
        }

        using (SQLiteCommand countCmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM bookings WHERE listingID = @listingId", conn))
        {
            countCmd.Parameters.AddWithValue("@listingId", listingId);
            long booked = (long)countCmd.ExecuteScalar()!;

            using SQLiteCommand capCmd = new SQLiteCommand(
                "SELECT capacity FROM listings WHERE listingID = @listingId", conn);
            capCmd.Parameters.AddWithValue("@listingId", listingId);
            int capacity = Convert.ToInt32(capCmd.ExecuteScalar()!);

            if (booked >= capacity)
                throw new InvalidOperationException("This listing is fully booked.");
        }

        using SQLiteCommand cmd = new SQLiteCommand(
            "INSERT INTO bookings (UUID, listingID, bookingStatus) VALUES (@uuid, @listingId, 'Confirmed')", conn);
        cmd.Parameters.AddWithValue("@uuid", uuid);
        cmd.Parameters.AddWithValue("@listingId", listingId);
        cmd.ExecuteNonQuery();
    }

    public List<Booking?> GetBookings(int id)
    {
        List<Booking?> bookings = new List<Booking?>();
        using SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand command = new SQLiteCommand(
            "SELECT * FROM bookings JOIN listings ON listings.listingID = bookings.listingID " +
            "WHERE bookings.UUID = @id", myConn);
        command.Parameters.AddWithValue("@id", id);

        using SQLiteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Booking booking = MapBooking(reader);
            bookings.Add(booking);
        }

        return bookings;
    }
    
    private static Booking MapBooking(SQLiteDataReader reader)
    {
        ListingStatus.TryParse(reader["bookingStatus"].ToString(), out ListingStatus bookingStatus);
        return new Booking
        {
            BookingId = Convert.ToInt32(reader["bookingID"]),
            UUID = Convert.ToInt32(reader["UUID"]),
            ListingId = Convert.ToInt32(reader["listingID"]),
            BookingStatus = bookingStatus,
            Category = ListingService.ParseListingCategory(reader),
            Title = reader["title"].ToString()!,
            Description = reader["description"].ToString()!,
            TransportMethod = reader["transportMethod"].ToString()!,
            Origin = reader["origin"].ToString()!,
            Destination = reader["destination"].ToString()!,
            Date = DateTime.Parse(reader["date"].ToString()!),
            Duration = Convert.ToInt32(reader["duration"]),
            DurationType = reader["durationType"].ToString()!,
            Capacity = Convert.ToInt32(reader["capacity"]),
            CapacityUnit = ListingService.ParseListingCapacityUnit(reader),
            Price = Convert.ToDecimal(reader["price"]),
            PriceUnit = ListingService.ParseListingPriceUnit(reader),
            ListingStatus = ListingService.ParseListingStatus(reader)
        };
    }
    
}
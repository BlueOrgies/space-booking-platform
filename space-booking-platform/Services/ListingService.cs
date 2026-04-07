using System.Data.SQLite;
using space_booking_platform.Models;
using Spectre.Console;

namespace space_booking_platform.Services;

public class ListingService
{
    public static void AddListingToTable(Listings listing)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        string sql =
            "INSERT INTO listings(type, title, description, transportMethod, origin, destination, date, " +
            "duration, durationType, capacity, capacityUnit, price, priceUnit, createdAt, listingStatus) VALUES (" +
            $"'{listing.Category}'," +
            $"'{listing.Title}'," +
            $"'{listing.Description}'," +
            $"'{listing.TransportMethod}'," +
            $"'{listing.Origin}'," +
            $"'{listing.Destination}'," +
            $"'{listing.Date}'," +
            $"'{listing.Duration}'," +
            $"'{listing.DurationType}'," +
            $"'{listing.Capacity}'," +
            $"'{listing.CapacityUnit}'," +
            $"'{listing.Price}'," +
            $"'{listing.PriceUnit}'," +
            $"'{listing.CreatedAt}'," +
            $"'{listing.ListingStatus}')";

        SQLiteCommand command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();

        myConn.Close();
    }
    
    public static void EditListingInDb(string edit, string newData, int listingId)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        string sql = $"UPDATE listings SET '{edit}' = '{newData}' WHERE listingID = '{listingId}'";
        SQLiteCommand command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();
        myConn.Close();
        AnsiConsole.MarkupLine("[bold]\nListing updated.[/]");
        
        myConn.Close();
    }
    
    
}
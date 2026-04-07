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
    
    public static void ShowOverview(string sql)
    {
        bool exists = false;
        SQLiteConnection myConn = Database.ConnectToDb();

        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);
  
        table.AddColumn("Category");
        table.AddColumn("Title");
        table.AddColumn("Origin");
        table.AddColumn("Destination");
        //table.AddColumn("Date");
        table.AddColumn("Status");
        
        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using (SQLiteDataReader dataReader = readThis.ExecuteReader())
        {
            while (dataReader.Read())
            {
                string? category = dataReader["type"].ToString();
                string? title = dataReader["title"].ToString();
                string? origin = dataReader["origin"].ToString();
                string? destination = dataReader["destination"].ToString();
                //string? date = dataReader["date"].ToString(); //TODO: not valid DateTime format 
                string? status = dataReader["listingStatus"].ToString();
  
                table.AddRow(category, title, origin, destination, status);

                exists = true;
            }
        }
        AnsiConsole.Write(table);
        
        if (!exists)
        {
            AnsiConsole.MarkupLine("There is nothing to show.");
        }

        myConn.Close();
    }
    
    //TODO: Edit this to show by UUID when this is implemented
    public static string ShowListings(int listingId)
    {
        string sql = "SELECT * FROM listings " +
                     $"WHERE listingID = '{listingId}' ";

        return sql;
    }
    
    //TODO: Edit this to show by UUID 
    public static string ShowBookings(int bookingId)
    {
        string sql = "SELECT * FROM bookings " +
                     "JOIN listings ON listings.listingID = bookings.listingID " +
                     $"WHERE bookingID = '{bookingId}' ";

        return sql;
    }
}
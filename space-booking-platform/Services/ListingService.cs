using System.Data.SQLite;
using System.Globalization;
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
            $"'{listing.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}'," +
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

        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());

        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using (SQLiteDataReader dataReader = readThis.ExecuteReader())
        {
            while (dataReader.Read())
            {
                string? category = dataReader["type"].ToString();
                string? title = dataReader["title"].ToString();
                string? origin = dataReader["origin"].ToString();
                string? destination = dataReader["destination"].ToString();
                string? date = dataReader["date"].ToString(); 
                string? status = dataReader["listingStatus"].ToString();

                table.AddRow(category, title, origin, destination, date, status);

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
    public static string ShowListings()
    {
        string sql = "SELECT * FROM listings ";

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

    public static void ShowMyListings()
    {
        //TODO: Change sql to show only logged in users listing 
        string sql = "SELECT * FROM listings";

        SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using (SQLiteDataReader dataReader = readThis.ExecuteReader())
        {
            while (dataReader.Read())
            {
                string? category = dataReader["type"].ToString();
                string? title = dataReader["title"].ToString();
                string? description = dataReader["description"].ToString();
                string? transportMethod = dataReader["transportMethod"].ToString();
                string? origin = dataReader["origin"].ToString();
                string? destination = dataReader["destination"].ToString();
                string? date = dataReader["date"].ToString();
                string? duration = dataReader["duration"].ToString();
                string? durationType = dataReader["durationType"].ToString();
                string? capacity = dataReader["capacity"].ToString();
                string? capacityUnit = dataReader["capacityUnit"].ToString();
                string? price = dataReader["price"].ToString();
                string? priceUnit = dataReader["priceUnit"].ToString();
                string? status = dataReader["listingStatus"].ToString();

                var grid = new Grid();
                grid.AddColumn();
                grid.AddColumn();

                grid.AddRow("", "");
                grid.AddRow("Category:", $"{category}");
                grid.AddRow("Title:", $"{title}");
                grid.AddRow("Description:", $"{description}");
                grid.AddRow("Transport method:", $"{transportMethod}");
                grid.AddRow("Origin:", $"{origin}");
                grid.AddRow("Destination:", $"{destination}");
                grid.AddRow("Date:", $"{date}");
                grid.AddRow("Duration:", $"{duration} {durationType}");
                grid.AddRow("Capacity:", $"{capacity} {capacityUnit}");
                grid.AddRow("Price:", $"{price} {priceUnit}");
                grid.AddRow("Status:", $"{status}");

                AnsiConsole.Write(grid);

            }
        }

        myConn.Close();
    }

    public static int ChooseListing()
    {
        //TODO: Change sql to only include listings from current user 
        var listings = new Dictionary<string, int>();

        string sql = "SELECT * FROM listings ";
        SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using SQLiteDataReader dataReader = readThis.ExecuteReader();
        while (dataReader.Read())
        {
            string title = dataReader["title"].ToString();
            int listingId = Convert.ToInt32(dataReader["listingID"]);
            listings.Add(title, listingId);
        }
        myConn.Close();
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which listing would you like to edit?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(listings.Keys));

        return listings.GetValueOrDefault(choice, -1);
    }
}

        
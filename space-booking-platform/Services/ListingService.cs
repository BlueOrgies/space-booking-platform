using System.Data.SQLite;
using space_booking_platform.Models;
using Spectre.Console;

namespace space_booking_platform.Services;

public class ListingService(AppState state)
{
    public Listings CreateListing(int uuid, ListingCategory category, string title, string description, string transportMethod, 
        string origin, string destination, DateTime date, int duration, string durationType, int capacity, 
        ListingCapacityUnit capacityUnit, decimal price, string priceUnit, DateTime createdAt, ListingStatus listingStatus)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        SQLiteCommand command = new SQLiteCommand(
            "INSERT INTO listings(uuid, type, title, description, transportMethod, origin, destination, date, " +
            "duration, durationType, capacity, capacityUnit, price, priceUnit, createdAt, listingStatus) VALUES (" +
            "@uuid, @type, @title, @description, @transportMethod, @origin, @destination, @date, @duration, @durationType, " +
            "@capacity, @capacityUnit, @price, @priceUnit, @createdAt, @listingStatus)", myConn);

        command.Parameters.AddWithValue("@uuid", uuid);
        command.Parameters.AddWithValue("@type", category);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@transportMethod", transportMethod);
        command.Parameters.AddWithValue("@origin", origin);
        command.Parameters.AddWithValue("@destination", destination);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@duration", duration);
        command.Parameters.AddWithValue("@durationType", durationType);
        command.Parameters.AddWithValue("@capacity", capacity);
        command.Parameters.AddWithValue("@capacityUnit", capacityUnit);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@priceUnit", priceUnit);
        command.Parameters.AddWithValue("@createdAt", createdAt);
        command.Parameters.AddWithValue("@listingStatus", listingStatus);

        command.ExecuteNonQuery();
        
        using SQLiteCommand fetchCmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingID = last_insert_rowid()", myConn);
        using SQLiteDataReader reader = fetchCmd.ExecuteReader();

        reader.Read();
        myConn.Close();
        return MapListings(reader);
    }

    public void EditListingInDb(string edit, string newData)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        string sql = $"UPDATE listings SET '{edit}' = '{newData}' WHERE listingID = '{state.currentListingID}'";
        SQLiteCommand command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();
        myConn.Close();
        AnsiConsole.MarkupLine("[bold]\nListing updated.[/]");

        myConn.Close();
    }

    public void ShowOverview(string sql)
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

    public string ShowUserListings()
    {
        string sql = "SELECT * FROM listings " +
                     $"WHERE listings.UUID = '{state.currentUUID}' " +
                     "ORDER BY listings.date " +
                     "LIMIT 5";

        return sql;
    }

    public string ShowUserBookings()
    {
        string sql = "SELECT * FROM bookings " +
                     "JOIN listings ON listings.listingID = bookings.listingID " +
                     $"WHERE listings.UUID = '{state.currentUUID}' " +
                     "ORDER BY listings.date " +
                     "LIMIT 5";

        return sql;
    }

    public void ShowMyListingsOrBookings(string sql)
    {
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
    private static Listings? MapListings(SQLiteDataReader reader) => new Listings
    {
        ListingId = Convert.ToInt32(reader["listingID"]),
        UUID = Convert.ToInt32(reader["UUID"]),
        Category = ParseListingCategory(reader),
        Title = reader["title"].ToString()!,
        Description = reader["description"].ToString()!,
        TransportMethod = reader["transportMethod"].ToString()!,
        Origin = reader["origin"].ToString()!,
        Destination = reader["destination"].ToString()!,
        Date = DateTime.Parse(reader["createdAt"].ToString()!), 
        Duration = Convert.ToInt32(reader["UUID"]),
        DurationType = reader["durationType"].ToString()!,
        Capacity = Convert.ToInt32(reader["UUID"]),
        CapacityUnit = ParseListingCapacityUnit(reader),
        Price = Convert.ToInt32(reader["UUID"]),
        PriceUnit = ParseListingPriceUnit(reader),
        ListingStatus = ParseListingStatus(reader)
    };

    private static ListingCategory ParseListingCategory(SQLiteDataReader reader)
    {
        ListingCategory.TryParse(reader["ListingCategory"].ToString(), out ListingCategory category);
        return category;
    }
    private static ListingCapacityUnit ParseListingCapacityUnit(SQLiteDataReader reader)
    {
        ListingCapacityUnit.TryParse(reader["ListingCategory"].ToString(), out ListingCapacityUnit unit);
        return unit;
    }
    private static ListingPriceUnit ParseListingPriceUnit(SQLiteDataReader reader)
    {
        ListingPriceUnit.TryParse(reader["ListingCategory"].ToString(), out ListingPriceUnit price);
        return price;
    }
    private static ListingStatus ParseListingStatus(SQLiteDataReader reader)
    {
        ListingStatus.TryParse(reader["ListingCategory"].ToString(), out ListingStatus status);
        return status;
    }

}

        
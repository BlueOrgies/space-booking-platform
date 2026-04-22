using System.Data.SQLite;
using space_booking_platform.Models;
using Spectre.Console;

namespace space_booking_platform.Services;

public class ListingService
{
    public Listings CreateListing(int uuid, ListingCategory category, string title, string description, string transportMethod, 
        string origin, string destination, DateTime date, int duration, string durationType, int capacity, 
        ListingCapacityUnit capacityUnit, decimal price, ListingPriceUnit priceUnit, DateTime createdAt, ListingStatus listingStatus)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        using (SQLiteCommand command = new SQLiteCommand(
                   "INSERT INTO listings(uuid, type, title, description, transportMethod, origin, destination, date, " +
                   "duration, durationType, capacity, capacityUnit, price, priceUnit, createdAt, listingStatus) VALUES (" +
                   "@uuid, @type, @title, @description, @transportMethod, @origin, @destination, @date, @duration, @durationType, " +
                   "@capacity, @capacityUnit, @price, @priceUnit, @createdAt, @listingStatus)", myConn))
        {
            command.Parameters.AddWithValue("@uuid", uuid);
            command.Parameters.AddWithValue("@type", category.ToString());
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@transportMethod", transportMethod);
            command.Parameters.AddWithValue("@origin", origin);
            command.Parameters.AddWithValue("@destination", destination);
            command.Parameters.AddWithValue("@date", date.ToString("o"));
            command.Parameters.AddWithValue("@duration", duration);
            command.Parameters.AddWithValue("@durationType", durationType);
            command.Parameters.AddWithValue("@capacity", capacity);
            command.Parameters.AddWithValue("@capacityUnit", capacityUnit.ToString());
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@priceUnit", priceUnit.ToString());
            command.Parameters.AddWithValue("@createdAt", createdAt.ToString("o"));
            command.Parameters.AddWithValue("@listingStatus", listingStatus.ToString());
            command.ExecuteNonQuery();
        }

        using SQLiteCommand fetchCmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingID = last_insert_rowid()", myConn);
        using SQLiteDataReader reader = fetchCmd.ExecuteReader();
        reader.Read();
        var listing = MapListings(reader);
        myConn.Close();
        return listing;
    }

    public void EditListing(int id, string edit, string newData)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand command = new SQLiteCommand(
            $"UPDATE listings SET {edit} = @newData WHERE listingID = @id", myConn);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@newData", newData);
        //command.Parameters.AddWithValue("@edit", edit);
        command.ExecuteNonQuery();
        AnsiConsole.MarkupLine("[bold]\nListing updated.[/]");
    }

    

    public List<Listings> GetListingsById(int id, int limit, int offset)
    {
        List<Listings> listings = new List<Listings>();
        using SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand command = new SQLiteCommand(
            "SELECT * FROM listings WHERE UUID = @id ORDER BY date LIMIT @limit OFFSET @offset", myConn);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@offset", offset);
        command.Parameters.AddWithValue("@limit", limit);

        using SQLiteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            listings.Add(MapListings(reader));

        return listings;
    }

    public List<Listings> GetAllListings(int offset)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();
        var listings = new List<Listings>();

        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingStatus = 'Active' ORDER BY date LIMIT 10 OFFSET @offset",
            myConn);
        cmd.Parameters.AddWithValue("@offset", offset);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            listings.Add(MapListings(reader));

        return listings;
    }

    public Listings? GetListingById(int listingId)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingID = @id", myConn);
        cmd.Parameters.AddWithValue("@id", listingId);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapListings(reader);
    }

    public List<Listings> SearchListings(string keyword, ListingCategory? category)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();
        var listings = new List<Listings>();

        string sql = "SELECT * FROM listings WHERE listingStatus = 'Active' " +
                     "AND (title LIKE @kw OR origin LIKE @kw OR destination LIKE @kw)";
        if (category.HasValue)
            sql += " AND type = @category";
        sql += " ORDER BY date";

        using SQLiteCommand cmd = new SQLiteCommand(sql, myConn);
        cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        if (category.HasValue)
            cmd.Parameters.AddWithValue("@category", category.Value.ToString());

        using SQLiteDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            listings.Add(MapListings(reader));

        return listings;
    }

    private static void SyncPastListingStatuses(SQLiteConnection connection)
    {
        var expiredListingIds = new List<int>();

        using (SQLiteCommand selectCommand = new SQLiteCommand(
                   "SELECT listingID, date FROM listings WHERE listingStatus = @status", connection))
        {
            selectCommand.Parameters.AddWithValue("@status", nameof(ListingStatus.Active));

            using SQLiteDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                DateTime listingDate = DateTime.Parse(reader["date"].ToString()!);
                if (listingDate < DateTime.Now)
                    expiredListingIds.Add(Convert.ToInt32(reader["listingID"]));
            }
        }

        if (expiredListingIds.Count == 0)
            return;

        using SQLiteTransaction transaction = connection.BeginTransaction();
        using SQLiteCommand updateCommand = new SQLiteCommand(
            "UPDATE listings SET listingStatus = @status WHERE listingID = @listingId", connection, transaction);
        updateCommand.Parameters.Add("@status", System.Data.DbType.String).Value = nameof(ListingStatus.Past);
        SQLiteParameter listingIdParameter = updateCommand.Parameters.Add("@listingId", System.Data.DbType.Int32);

        foreach (int listingId in expiredListingIds)
        {
            listingIdParameter.Value = listingId;
            updateCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }


    private static Listings MapListings(SQLiteDataReader reader) => new Listings
    {
        ListingId = Convert.ToInt32(reader["listingID"]),
        UUID = Convert.ToInt32(reader["UUID"]),
        Category = ParseListingCategory(reader),
        Title = reader["title"].ToString()!,
        Description = reader["description"].ToString()!,
        TransportMethod = reader["transportMethod"].ToString()!,
        Origin = reader["origin"].ToString()!,
        Destination = reader["destination"].ToString()!,
        Date = DateTime.Parse(reader["date"].ToString()!),
        Duration = Convert.ToInt32(reader["duration"]),
        DurationType = reader["durationType"].ToString()!,
        Capacity = Convert.ToInt32(reader["capacity"]),
        CapacityUnit = ParseListingCapacityUnit(reader),
        Price = Convert.ToDecimal(reader["price"]),
        PriceUnit = ParseListingPriceUnit(reader),
        ListingStatus = ParseListingStatus(reader),
        CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!)
    };

    public static ListingCategory ParseListingCategory(SQLiteDataReader reader)
    {
        ListingCategory.TryParse(reader["type"].ToString(), out ListingCategory category);
        return category;
    }
    public static ListingCapacityUnit ParseListingCapacityUnit(SQLiteDataReader reader)
    {
        ListingCapacityUnit.TryParse(reader["capacityUnit"].ToString(), out ListingCapacityUnit unit);
        return unit;
    }
    public static ListingPriceUnit ParseListingPriceUnit(SQLiteDataReader reader)
    {
        ListingPriceUnit.TryParse(reader["priceUnit"].ToString(), out ListingPriceUnit price);
        return price;
    }
    public static ListingStatus ParseListingStatus(SQLiteDataReader reader)
    {
        ListingStatus.TryParse(reader["listingStatus"].ToString(), out ListingStatus status);
        return status;
    }
    
    /// <summary>
    /// Deprecated method 
    /// </summary>
    /// <param name="sql"></param>
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
    /// <summary>
    /// Deprecated
    /// </summary>
    /// <param name="sql"></param>
    public void ShowOverview(string sql)
    {
        bool exists = false;
        SQLiteConnection myConn = Database.ConnectToDb();

        var table = new Table()
            .SimpleBorder()
            .BorderColor(Color.Green);

        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());

        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using SQLiteDataReader dataReader = readThis.ExecuteReader();
        while (dataReader.Read())
        {
            Listings listing = MapListings(dataReader);

            table.AddRow(listing.Category.ToString(), listing.Title, listing.Origin,
                listing.Destination, listing.Date.ToString("o"), listing.ListingStatus.ToString());

            exists = true;
        }

        if (!exists)
        {
            AnsiConsole.MarkupLine("There is nothing to show.");
            myConn.Close();
        }
        else
        {
            AnsiConsole.Write(table);
            myConn.Close();
        }
    }
    /// <summary>
    /// Deprecated
    /// </summary>
    /// <returns></returns>
    public string ShowUserListings(int id)
    {
        string sql = "SELECT * FROM listings " +
                     $"WHERE listings.UUID = '{id}' " +
                     "ORDER BY listings.date " +
                     "LIMIT 5";

        return sql;
    }
    /// <summary>
    /// Deprecated
    /// </summary>
    /// <returns></returns>
    public string ShowUserBookings(int id)
    {
        string sql = "SELECT * FROM bookings " +
                     "JOIN listings ON listings.listingID = bookings.listingID " +
                     $"WHERE bookings.UUID = '{id}' " +
                     "ORDER BY listings.date " +
                     "LIMIT 5";

        return sql;
    }
}

        
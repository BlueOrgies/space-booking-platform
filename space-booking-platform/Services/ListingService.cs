using System.Data.SQLite;
using space_booking_platform.Models;
using Spectre.Console;

namespace space_booking_platform.Services;

public class ListingService
{
    /// <summary>
    /// Creates a new listing in the system.
    /// </summary>
    /// <param name="uuid">The identifier of the user creating the listing.</param>
    /// <param name="category">The category of the listing.</param>
    /// <param name="title">The title of the listing.</param>
    /// <param name="description">A detailed description of the listing.</param>
    /// <param name="transportMethod">The method of transport (if applicable).</param>
    /// <param name="origin">The starting location of the trip.</param>
    /// <param name="destination">The destination location of the trip.</param>
    /// <param name="date">The date of the listing.</param>
    /// <param name="duration">The duration of the service.</param>
    /// <param name="durationType">The unit of duration (e.g., Days, Hours).</param>
    /// <param name="capacity">The maximum capacity of the listing.</param>
    /// <param name="capacityUnit">The unit of capacity (e.g., Seats, MaxWeight).</param>
    /// <param name="price">The price of the listing.</param>
    /// <param name="priceUnit">The unit of price (e.g., Euros, EurosPerKg).</param>
    /// <param name="createdAt">The date and time the listing was created.</param>
    /// <param name="listingStatus">The initial status of the listing.</param>
    /// <param name="location">The physical location (for accommodations or activities).</param>
    /// <param name="petsAllowed">Whether pets are allowed (for accommodations).</param>
    /// <param name="luggageIncluded">Whether luggage is included (for transportation).</param>
    /// <param name="hazardousMaterialsAllowed">Whether hazardous materials are allowed (for freight).</param>
    /// <param name="minAge">The minimum age required (for activities).</param>
    /// <returns>The created listing object.</returns>
    public Listings CreateListing(int uuid, ListingCategory category, string title, string description,
        string transportMethod, string origin, string destination, DateTime date, int duration, string durationType,
        int capacity, ListingCapacityUnit capacityUnit, decimal price, ListingPriceUnit priceUnit,
        DateTime createdAt, ListingStatus listingStatus,
        string location = "", bool petsAllowed = false, bool luggageIncluded = false,
        bool hazardousMaterialsAllowed = false, int minAge = 0)
    {
        SQLiteConnection myConn = Database.ConnectToDb();

        using (SQLiteCommand command = new SQLiteCommand(
                   "INSERT INTO listings(uuid, type, title, description, transportMethod, origin, destination, date, " +
                   "duration, durationType, capacity, capacityUnit, price, priceUnit, createdAt, listingStatus, " +
                   "location, petsAllowed, luggageIncluded, hazardousMaterialsAllowed, minAge) VALUES (" +
                   "@uuid, @type, @title, @description, @transportMethod, @origin, @destination, @date, @duration, @durationType, " +
                   "@capacity, @capacityUnit, @price, @priceUnit, @createdAt, @listingStatus, " +
                   "@location, @petsAllowed, @luggageIncluded, @hazardousMaterialsAllowed, @minAge)", myConn))
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
            command.Parameters.AddWithValue("@location", location);
            command.Parameters.AddWithValue("@petsAllowed", petsAllowed);
            command.Parameters.AddWithValue("@luggageIncluded", luggageIncluded);
            command.Parameters.AddWithValue("@hazardousMaterialsAllowed", hazardousMaterialsAllowed);
            command.Parameters.AddWithValue("@minAge", minAge);
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

    /// <summary>
    /// Edits an existing listing's information.
    /// </summary>
    /// <param name="id">The identifier of the listing to edit.</param>
    /// <param name="edit">The field to edit.</param>
    /// <param name="newData">The new value for the field.</param>
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

    /// <summary>
    /// Cancels a listing.
    /// </summary>
    /// <param name="listingId">The identifier of the listing to cancel.</param>
    /// <param name="organizerUuid">The identifier of the organizer canceling the listing.</param>
    /// <exception cref="InvalidOperationException">Thrown if the listing is not found or not owned by the user.</exception>
    public void CancelListing(int listingId, int organizerUuid)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();

        using SQLiteCommand command = new SQLiteCommand(
            "UPDATE listings SET listingStatus = @status WHERE listingID = @listingId AND UUID = @uuid", myConn);
        command.Parameters.AddWithValue("@status", ListingStatus.Cancelled.ToString());
        command.Parameters.AddWithValue("@listingId", listingId);
        command.Parameters.AddWithValue("@uuid", organizerUuid);

        int affectedRows = command.ExecuteNonQuery();
        if (affectedRows == 0)
            throw new InvalidOperationException("Only the listing organizer can cancel this listing.");
    }

    

    /// <summary>
    /// Retrieves a list of listings created by a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="limit">The maximum number of listings to return.</param>
    /// <param name="offset">The number of listings to skip.</param>
    /// <returns>A list of listings.</returns>
    public List<Listings> GetListingsByUserId(int id, int limit, int offset)
    {
        List<Listings> listings = new List<Listings>();
        using SQLiteConnection myConn = Database.ConnectToDb();
        SyncPastListingStatuses(myConn);

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

    /// <summary>
    /// Retrieves a list of active (upcoming) listings.
    /// </summary>
    /// <param name="offset">The number of listings to skip.</param>
    /// <returns>A list of active listings.</returns>
    public List<Listings> GetActiveListings(int offset)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();
        var listings = new List<Listings>();
        SyncPastListingStatuses(myConn);

        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingStatus = 'Upcoming' ORDER BY date LIMIT 10 OFFSET @offset",
            myConn);
        cmd.Parameters.AddWithValue("@offset", offset);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            listings.Add(MapListings(reader));

        return listings;
    }

    /// <summary>
    /// Retrieves a specific listing by its identifier.
    /// </summary>
    /// <param name="listingId">The identifier of the listing.</param>
    /// <returns>The listing if found; otherwise, null.</returns>
    public Listings? GetListingById(int listingId)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();
        SyncPastListingStatuses(myConn);

        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM listings WHERE listingID = @id", myConn);
        cmd.Parameters.AddWithValue("@id", listingId);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapListings(reader);
    }

    /// <summary>
    /// Searches for listings based on a keyword and optional category.
    /// </summary>
    /// <param name="keyword">The search keyword.</param>
    /// <param name="category">The optional category to filter by.</param>
    /// <returns>A list of matching listings.</returns>
    public List<Listings> SearchListings(string keyword, ListingCategory? category)
    {
        using SQLiteConnection myConn = Database.ConnectToDb();
        var listings = new List<Listings>();
        SyncPastListingStatuses(myConn);

        string sql = "SELECT * FROM listings WHERE listingStatus = 'Upcoming' " +
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
            selectCommand.Parameters.AddWithValue("@status", nameof(ListingStatus.Upcoming));

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


    private static Listings MapListings(SQLiteDataReader reader)
    {
        var category = ParseListingCategory(reader);

        Listings listing = category switch
        {
            ListingCategory.Accommodation => new Accommodation(),
            ListingCategory.PassengerTransportation => new PassengerTransportation(),
            ListingCategory.FreightHaul => new FreightHaul(),
            ListingCategory.Activity => new Activity(),
            _ => new Other()
        };

        listing.ListingId = Convert.ToInt32(reader["listingID"]);
        listing.UUID = Convert.ToInt32(reader["UUID"]);
        listing.Category = ParseListingCategory(reader);
        listing.Title = reader["title"].ToString()!;
        listing.Description = reader["description"].ToString()!;
        listing.Date = DateTime.Parse(reader["date"].ToString()!);
        listing.Duration = Convert.ToInt32(reader["duration"]);
        listing.DurationType = reader["durationType"].ToString()!;
        listing.Capacity = Convert.ToInt32(reader["capacity"]);
        listing.CapacityUnit = ParseListingCapacityUnit(reader);
        listing.Price = Convert.ToDecimal(reader["price"]);
        listing.PriceUnit = ParseListingPriceUnit(reader);
        listing.ListingStatus = ParseListingStatus(reader);
        listing.CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!);

        switch (listing)
        {
            case Accommodation a:
                a.Location = reader["location"].ToString()!;
                a.PetsAllowed = Convert.ToBoolean(reader["petsAllowed"]);
                break;
            case PassengerTransportation pt:
                pt.LuggageIncluded = Convert.ToBoolean(reader["luggageIncluded"]);
                pt.TransportMethod = reader["transportMethod"].ToString()!;
                pt.Origin = reader["origin"].ToString()!;
                pt.Destination = reader["destination"].ToString()!;
                break;
            case FreightHaul fh:
                fh.HazardousMaterialsAllowed = Convert.ToBoolean(reader["hazardousMaterialsAllowed"]);
                fh.TransportMethod = reader["transportMethod"].ToString()!;
                fh.Origin = reader["origin"].ToString()!;
                fh.Destination = reader["destination"].ToString()!;
                break;
            case Activity act:
                act.Location = reader["location"].ToString()!;
                act.MinAge = Convert.ToInt32(reader["minAge"]);
                break;
        }

        return listing;
    }

    /// <summary>
    /// Parses the listing category from a database reader.
    /// </summary>
    /// <param name="reader">The SQLite data reader.</param>
    /// <returns>The parsed ListingCategory.</returns>
    public static ListingCategory ParseListingCategory(SQLiteDataReader reader)
    {
        ListingCategory.TryParse(reader["type"].ToString(), out ListingCategory category);
        return category;
    }
    /// <summary>
    /// Parses the listing capacity unit from a database reader.
    /// </summary>
    /// <param name="reader">The SQLite data reader.</param>
    /// <returns>The parsed ListingCapacityUnit.</returns>
    public static ListingCapacityUnit ParseListingCapacityUnit(SQLiteDataReader reader)
    {
        ListingCapacityUnit.TryParse(reader["capacityUnit"].ToString(), out ListingCapacityUnit unit);
        return unit;
    }
    /// <summary>
    /// Parses the listing price unit from a database reader.
    /// </summary>
    /// <param name="reader">The SQLite data reader.</param>
    /// <returns>The parsed ListingPriceUnit.</returns>
    public static ListingPriceUnit ParseListingPriceUnit(SQLiteDataReader reader)
    {
        ListingPriceUnit.TryParse(reader["priceUnit"].ToString(), out ListingPriceUnit price);
        return price;
    }
    /// <summary>
    /// Parses the listing status from a database reader.
    /// </summary>
    /// <param name="reader">The SQLite data reader.</param>
    /// <returns>The parsed ListingStatus.</returns>
    public static ListingStatus ParseListingStatus(SQLiteDataReader reader)
    {
        string? rawStatus = reader["listingStatus"].ToString();
        if (string.Equals(rawStatus, nameof(ListingStatus.Past), StringComparison.OrdinalIgnoreCase))
            return ListingStatus.Past;

        ListingStatus.TryParse(rawStatus, out ListingStatus status);
        return status;
    }
    
    /// <summary>
    /// Deprecated method 
    /// </summary>
    /// <param name="sql"></param>
    /// <summary>
    /// Displays listings or bookings in a table format based on a SQL query.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
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
    /// <summary>
    /// Displays an overview of listings based on a SQL query.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
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

            string originCol = listing is PassengerTransportation pt ? pt.Origin
                : listing is FreightHaul fh ? fh.Origin : string.Empty;
            string destinationCol = listing is PassengerTransportation pt2 ? pt2.Destination
                : listing is FreightHaul fh2 ? fh2.Destination : string.Empty;

            table.AddRow(listing.Category.ToString(), listing.Title, originCol,
                destinationCol, listing.Date.ToString("o"), listing.ListingStatus.ToString());

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
    /// <summary>
    /// Shows the listings for a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>The name of the next view to display.</returns>
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
    /// <summary>
    /// Shows the bookings for a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>The name of the next view to display.</returns>
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

        
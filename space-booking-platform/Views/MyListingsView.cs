using System.Data.SQLite;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    //TODO: Move parts to ListingService and add so it only shows own listings 
    public static string? Display()
    {
        AnsiConsole.Clear();
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
                //string? date = dataReader["date"].ToString();
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
                //grid.AddRow("Date:", $"{date}");
                grid.AddRow("Duration:", $"{duration} {durationType}");
                grid.AddRow("Capacity:", $"{capacity} {capacityUnit}");
                grid.AddRow("Price:", $"{price} {priceUnit}");
                grid.AddRow("Status:", $"{status}");

                AnsiConsole.Write(grid);

            }
        }

        myConn.Close();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhere would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Go back to profile",
                    "Go back to main menu", "Quit"));

        return choice switch
        {
            "Go back to profile" => "OrganizerView",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
    }
}
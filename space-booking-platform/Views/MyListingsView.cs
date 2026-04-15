using System.Data.SQLite;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]My listings[/]");

        SQLiteConnection myConn = Database.ConnectToDb();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        
        //TODO: Fix sql
        string sql = "SELECT * FROM listings";

        using SQLiteCommand readThis = new SQLiteCommand(sql, myConn);
        using (SQLiteDataReader dataReader = readThis.ExecuteReader())
        {
            while (dataReader.Read())
            {
                string? id = dataReader["listingID"].ToString();
                string? category = dataReader["type"].ToString();
                string? title = dataReader["title"].ToString();
                string? origin = dataReader["origin"].ToString();
                string? destination = dataReader["destination"].ToString();
                string? date = dataReader["date"].ToString();
                string? status = dataReader["listingStatus"].ToString();

                rows.Add($"{category} | {title} | {origin} | {destination} | {date} | {status}", id);
            }
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhere would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoiceGroup("", rows.Keys.ToArray())
                .AddChoiceGroup("", "Next page", "Go back to profile", "Go back to main menu", "Quit"));

        if (rows.TryGetValue(choice, out string? value))
        {
            state.currentListingID = int.Parse(value);
            return "ListingView";
        }
        //TODO: Add pagination
        return choice switch
        {
            "Go back to profile" => "ProfileView",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
    }
}
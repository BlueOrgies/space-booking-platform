using System.Data.SQLite;
using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]My listings[/]");

        ListingService ls = new ListingService();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        List<Listings?> listings = ls.GetListings(state.currentUUID);

        foreach (Listings? listing in listings)
        {
            rows.Add($"{listing.Category} | {listing.Title} | {listing.Origin} | {listing.Destination}" +
                     $" | {listing.Date} | {listing.ListingStatus}", listing.ListingId.ToString());
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhere would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoiceGroup("", rows.Keys.ToArray())
                .AddChoiceGroup("", "Next page", "Go back to profile", 
                                "Go back to main menu", "Quit"));

        if (!rows.TryGetValue(choice, out string? value))
            return choice switch
            {
                "Go back to profile" => "ProfileView",
                "Go back to main menu" => "Home",
                _ => null // Quit
            };
        state.currentListingID = int.Parse(value);
        return "Listing";
        //TODO: Add pagination
    }
}
using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    private const int Limit = 11;
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]My listings[/]");

        ListingService ls = new ListingService();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        List<Listings> listings = ls.GetListingsById(state.CurrentUUID, Limit, state.Offset);

        if (listings.Count > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                Listings listing = listings[i];
                rows.Add($"{listing.Category} | {listing.Title} | {listing.Origin} | {listing.Destination}" +
                         $" | {listing.Date} | {listing.ListingStatus}", listing.ListingId.ToString());
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No listings available[/]");
        }

        var choices = new List<string> { "Go back to profile", "Go back to main menu", "Quit" };
        
        if (listings.Count > 10) //Limit is 11, so if there is more than 10 (something to put on the next page) you get next page option
        {
            choices.Insert(0, "Next page");
        }
        if (state.Offset > 0)
        {
            choices.Insert(0, "Previous page");
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoiceGroup("", rows.Keys.ToArray())
                .AddChoiceGroup("", choices));

        if (rows.TryGetValue(choice, out string? value))
        {
            state.CurrentListingID = int.Parse(value);
            return "Listing";
        }

        switch (choice)
        {
            case "Next page":
                state.Offset += 10;
                return "MyListings";
            case "Previous page":
                state.Offset -= 10;
                return "MyListings";
            case "Go back to main menu":
                return "Home";
            case "Go back to profile":
                return "ProfileView";
            default:
                return null;
        }
    }
}

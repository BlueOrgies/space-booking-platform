using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    private const int Limit = 10;
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]My listings[/]").RuleStyle("green"));

        ListingService ls = new ListingService();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        List<Listings> listings = ls.GetListingsById(state.CurrentUUID, Limit, state.Offset);

        if (listings.Count > 0)
        {
            foreach (var listing in listings)
            {
                rows.Add($"{listing.Category} | {listing.Title} | {listing.Origin} | {listing.Destination}" +
                         $" | {listing.Date} | {listing.ListingStatus}", listing.ListingId.ToString());
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No listings available[/]");
        }

        var choices = new List<string> { "Go back", "Main menu" };
        
        if (listings.Count == 10) 
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
            case "Main menu":
                return "Home";
            case "Go back":
                return "OrganizerView";
            default:
                return null;
        }
    }
}

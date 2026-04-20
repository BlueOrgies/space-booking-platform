using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class MyListingsView(AppState state)
{
    private const int PageSize = 5;
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]My listings[/]");

        ListingService ls = new ListingService();

        Dictionary<string, string> rows = new Dictionary<string, string>();
        List<Listings> listings = ls.GetListings(state.CurrentUUID);

        int startIndex = state.CurrentPage * PageSize;
        int endIndex = Math.Min(startIndex + PageSize, listings.Count);
        int totalPages = (int)Math.Ceiling((double)listings.Count / PageSize);

        for (int i = startIndex; i < endIndex; i++)
        {
            Listings listing = listings[i];
            rows.Add($"{listing.Category} | {listing.Title} | {listing.Origin} | {listing.Destination}" +
                     $" | {listing.Date} | {listing.ListingStatus}", listing.ListingId.ToString());
        }

        var choices = new List<string> { "Go back to profile", "Go back to main menu", "Quit" };

        if (endIndex < listings.Count)
        {
            choices.Insert(0, "Next page");
        }
        if (startIndex > 0)
        {
            choices.Insert(0, "Previous page");
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"\nShowing page {state.CurrentPage + 1} of {totalPages}. Where would you like to go?")
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
                state.CurrentPage++;
                return "MyListings";
            case "Previous page":
                state.CurrentPage--;
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

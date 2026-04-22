using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

class BrowseListingsView(AppState state)
{
    private const int PageSize = 10;
    private const string PreviousPageChoice = "← Previous 10";
    private const string NextPageChoice = "→ Next 10";
    private const string SearchChoice = "Search Listings";
    private const string BackChoice = "Back to main menu";

    public string? Display()
    {
        var listingService = new ListingService();
        int offset = 0;

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold green]Browse Listings[/]").RuleStyle("green"));

            List<Listings> listings = listingService.GetAllListings(offset);
            Dictionary<string, int> listingMap = BuildListingMap(listings);
            List<string> navChoices = BuildNavigationChoices(offset, listings.Count);

            string choice = PromptForChoice(listingMap, navChoices);
            string? route = HandleChoice(choice, listingMap, ref offset);
            if (route != null)
                return route;
        }
    }

    private static Dictionary<string, int> BuildListingMap(IEnumerable<Listings> listings)
    {
        var listingMap = new Dictionary<string, int>();
        foreach (Listings listing in listings)
        {
            listingMap[BuildListingLabel(listing)] = listing.ListingId;
        }

        return listingMap;
    }

    private static string BuildListingLabel(Listings listing)
    {
        return $"[[{listing.Category}]] {Markup.Escape(listing.Title)} | {Markup.Escape(listing.Origin)} → {Markup.Escape(listing.Destination)} | {listing.Date:yyyy-MM-dd} | {listing.Price} {listing.PriceUnit}";
    }

    private static List<string> BuildNavigationChoices(int offset, int listingCount)
    {
        var navChoices = new List<string>();
        if (offset > 0)
            navChoices.Add(PreviousPageChoice);
        if (listingCount == PageSize)
            navChoices.Add(NextPageChoice);

        navChoices.Add(SearchChoice);
        navChoices.Add(BackChoice);
        return navChoices;
    }

    private static string PromptForChoice(Dictionary<string, int> listingMap, List<string> navChoices)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Select a listing to view details:")
            .HighlightStyle(new Style(Color.Yellow));

        if (listingMap.Count > 0)
        {
            prompt.AddChoiceGroup("Listings", listingMap.Keys.ToArray());
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No listings available.[/]");
        }

        prompt.AddChoiceGroup("Navigation", navChoices.ToArray());
        return AnsiConsole.Prompt(prompt);
    }

    private string? HandleChoice(string choice, Dictionary<string, int> listingMap, ref int offset)
    {
        if (listingMap.TryGetValue(choice, out int listingId))
        {
            state.CurrentListingID = listingId;
            return "Listing";
        }

        switch (choice)
        {
            case PreviousPageChoice:
                offset -= PageSize;
                return null;
            case NextPageChoice:
                offset += PageSize;
                return null;
            case SearchChoice:
                return "SearchListings";
            case BackChoice:
                return "Home";
            default:
                return null;
        }
    }
}

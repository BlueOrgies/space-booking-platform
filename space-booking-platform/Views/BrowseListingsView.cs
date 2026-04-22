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

            List<Listings> listings = listingService.GetActiveListings(offset);

            var prompt = new SelectionPrompt<string>()
                .Title("Select a listing to view details:")
                .HighlightStyle(new Style(Color.Yellow));

            var listingMap = new Dictionary<string, int>();

            if (listings.Count > 0)
            {
                foreach (var listing in listings)
                {
                    string label = $"[[{listing.Category}]] {Markup.Escape(listing.Title)} | {Markup.Escape(listing.Origin)} → {Markup.Escape(listing.Destination)} | {listing.Date:yyyy-MM-dd} | {listing.Price} {listing.PriceUnit}";
                    listingMap[label] = listing.ListingId;
                }
                prompt.AddChoiceGroup("Listings", listingMap.Keys.ToArray());
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]No listings available.[/]");
            }

            var navChoices = new List<string>();
            if (offset > 0) navChoices.Add("← Previous 10");
            if (listings.Count == 10) navChoices.Add("→ Next 10");
            navChoices.Add("Search Listings");
            navChoices.Add("Back to main menu");

            prompt.AddChoiceGroup("Navigation", navChoices.ToArray());

            string choice = AnsiConsole.Prompt(prompt);

            if (listingMap.TryGetValue(choice, out int listingId))
            {
                state.CurrentListingID = listingId;
                return "Listing";
            }

            switch (choice)
            {
                case "← Previous 10":
                    offset -= 10;
                    break;
                case "→ Next 10":
                    offset += 10;
                    break;
                case "Search Listings":
                    return "SearchListings";
                case "Back to main menu":
                    return "Home";
            }
        }
    }
}

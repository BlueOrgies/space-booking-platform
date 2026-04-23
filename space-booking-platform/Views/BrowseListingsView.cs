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
                    string originDest = listing is PassengerTransportation ptB ? $"{Markup.Escape(ptB.Origin)} → {Markup.Escape(ptB.Destination)}"
                        : listing is FreightHaul fhB ? $"{Markup.Escape(fhB.Origin)} → {Markup.Escape(fhB.Destination)}" : string.Empty;
                    string label = $"[[{listing.Category}]] {Markup.Escape(listing.Title)}{(originDest.Length > 0 ? " | " + originDest : "")} | {listing.Date:yyyy-MM-dd} | {listing.Price} {listing.PriceUnit}";
                    listingMap[label] = listing.ListingId;
                }
                prompt.AddChoiceGroup("Listings", listingMap.Keys.ToArray());
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]No listings available.[/]");
            }

            var navChoices = new List<string>();
            if (offset > 0) navChoices.Add(PreviousPageChoice);
            if (listings.Count == 10) navChoices.Add(NextPageChoice);
            navChoices.Add(SearchChoice);

            prompt.AddChoiceGroup("\nNavigation", navChoices.ToArray());
            prompt.AddChoiceGroup("\n", BackChoice);

            string choice = AnsiConsole.Prompt(prompt);

            if (listingMap.TryGetValue(choice, out int listingId))
            {
                state.CurrentListingID = listingId;
                return "Listing";
            }

            switch (choice)
            {
                case PreviousPageChoice:
                    offset -= 10;
                    break;
                case NextPageChoice:
                    offset += 10;
                    break;
                case SearchChoice:
                    return "SearchListings";
                case BackChoice:
                    return "Home";
            }
        }
    }
}

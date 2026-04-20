using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

class SearchListingsView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Search Listings[/]").RuleStyle("green"));

        string keyword = AnsiConsole.Ask<string>("Search by keyword [grey](title, origin, destination)[/]: ");

        var categoryChoices = new List<string> { "All categories" };
        categoryChoices.AddRange(Enum.GetNames<ListingCategory>());

        string categoryChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Filter by category:")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(categoryChoices));

        ListingCategory? category = categoryChoice == "All categories"
            ? null
            : Enum.Parse<ListingCategory>(categoryChoice);

        var listingService = new ListingService();
        List<Listings> results = listingService.SearchListings(keyword, category);

        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Search Results[/]").RuleStyle("green"));

        if (results.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No listings found matching your search.[/]");
            AnsiConsole.WriteLine();
            var fallback = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices("Search again", "Browse all listings", "Back to main menu"));
            return fallback switch
            {
                "Search again"        => "SearchListings",
                "Browse all listings" => "BrowseListings",
                _                     => "Home"
            };
        }

        AnsiConsole.MarkupLine($"[grey]{results.Count} result(s) found.[/]");
        AnsiConsole.WriteLine();

        var listingMap = new Dictionary<string, int>();
        foreach (var listing in results)
        {
            string label = $"[[{listing.Category}]] {Markup.Escape(listing.Title)} | {Markup.Escape(listing.Origin)} → {Markup.Escape(listing.Destination)} | {listing.Date:yyyy-MM-dd} | {listing.Price} {listing.PriceUnit}";
            listingMap[label] = listing.ListingId;
        }

        var prompt = new SelectionPrompt<string>()
            .Title("Select a listing to view details:")
            .HighlightStyle(new Style(Color.Yellow))
            .AddChoiceGroup("Results", listingMap.Keys.ToArray())
            .AddChoiceGroup("Navigation", "Search again", "Browse all listings", "Back to main menu");

        string choice = AnsiConsole.Prompt(prompt);

        if (listingMap.TryGetValue(choice, out int listingId))
        {
            state.currentListingID = listingId;
            return "Listing";
        }

        return choice switch
        {
            "Search again"        => "SearchListings",
            "Browse all listings" => "BrowseListings",
            _                     => "Home"
        };
    }
}

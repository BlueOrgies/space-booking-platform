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

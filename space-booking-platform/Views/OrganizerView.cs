using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerView(AppState state)
{
    private const int Limit = 5;
    public string? Display()
    {
        AnsiConsole.Clear();
        
        ReviewService rs = new ReviewService();
        var choices = new List<string> {"Create listing", "Go back to main menu"};
        
        AnsiConsole.Write(new Rule($"[bold green]{state.CurrentUser}s profile: Organizer[/]").RuleStyle("green"));

        double rating = rs.GetAverageRating(state.CurrentUUID);
        if (rating > 0.0)
        {
            AnsiConsole.MarkupLine($"Average rating: [yellow]{rating}[/]");
        }

        if (ShowUpcomingListings() | ShowPastListings())
        {
            state.Offset = 0;
            choices.Insert(0, "View my listings");
        }

        if (ShowReviews())
        {
            choices.Insert(1, "View my reviews");
        }

        Console.WriteLine("");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));

        return choice switch
        {
            "View my listings" => "MyListings",
            "Create listing" => "CreateListing",
            "View my reviews" => "Review",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
    }

    private bool ShowUpcomingListings()
    {
        ListingService listingService = new ListingService();
        List<Listings> listings = listingService.GetListingsByUserId(state.CurrentUUID, Limit, 0);
        List<Listings> upcomingListings = new List<Listings>();
        upcomingListings.AddRange(listings.Where(listing => listing.ListingStatus != ListingStatus.Past));
        
        AnsiConsole.MarkupLine("\n[green]My upcoming listings[/]");
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);
        
        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());

        if (upcomingListings.Count > 0)
        {
            foreach (Listings listing in upcomingListings)
            {
                string origin = listing is PassengerTransportation ptU ? ptU.Origin : listing is FreightHaul fhU ? fhU.Origin : string.Empty;
                string destination = listing is PassengerTransportation ptU2 ? ptU2.Destination : listing is FreightHaul fhU2 ? fhU2.Destination : string.Empty;
                table.AddRow(listing.Category.ToString(), listing.Title, origin, destination,
                    listing.Date.ToString("o"), listing.ListingStatus.ToString());
            }
            AnsiConsole.Write(table);
            return true;
        }
        AnsiConsole.MarkupLine("[grey]No listings available[/]");
        return false;
    }

    private bool ShowPastListings()
    {
        ListingService listingService = new ListingService();
        List<Listings> listings = listingService.GetListingsByUserId(state.CurrentUUID, Limit, 0);
        List<Listings> pastListings = new List<Listings>();
        pastListings.AddRange(listings.Where(listing => listing.ListingStatus == ListingStatus.Past));
        
        AnsiConsole.MarkupLine("\n[green]My past listings[/]");
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);
        
        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        
        if (pastListings.Count > 0)
        {
            foreach (Listings listing in pastListings)
            {
                string origin = listing is PassengerTransportation ptP ? ptP.Origin : listing is FreightHaul fhP ? fhP.Origin : string.Empty;
                string destination = listing is PassengerTransportation ptP2 ? ptP2.Destination : listing is FreightHaul fhP2 ? fhP2.Destination : string.Empty;
                table.AddRow(listing.Category.ToString(), listing.Title, origin, destination,
                    listing.Date.ToString("o"));
            }
            AnsiConsole.Write(table);
            return true;
        }
        AnsiConsole.MarkupLine("[grey]No listings available[/]");
        return false;
    }

    private bool ShowReviews()
    {
        ReviewService reviewService = new ReviewService();
        
        AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);

        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Rating[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Comment[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());

        List<Review?> reviews = reviewService.GetLimitedReviews(state.CurrentUUID, Limit);
        if (reviews.Count > 0)
        {
            foreach (Review? review in reviews)
            {
                table.AddRow(review.Type, review.Title, review.Rating.ToString(),
                    review.Comment, review.CreatedAt.ToString("o"));
            }
            AnsiConsole.Write(table);
            return true;
        }
        AnsiConsole.MarkupLine("[grey]No reviews available[/]");
        return false;
    }
}
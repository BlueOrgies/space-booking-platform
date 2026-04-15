using System.Data.SQLite;
using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        
        ListingService ls = new ListingService();
        ReviewService rs = new ReviewService();
        var choices = new List<string> {"Create listing", "Go back to main menu", "Quit" };
        
        AnsiConsole.MarkupLine($"[bold green]=== {state.currentUser}s profile: Organizer ===[/]");

        AnsiConsole.MarkupLine("\n[green]My listings[/]");
        var table = new Table()
            .SimpleBorder()
            .BorderColor(Color.Green);
        
        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());
        
        List<Listings?> listings = ls.GetListings(state.currentUUID);
        switch (listings.Count)
        { 
            case > 5:
            {
                foreach (Listings? listing in listings.GetRange(0, 5))
                {
                    table.AddRow(listing.Category.ToString(), listing.Title, listing.Origin, listing.Destination,
                        listing.Date.ToString("o"), listing.ListingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Insert(0, "View my listings");
                break;
            }
            case > 0:
            {
                foreach (Listings listing in listings)
                {
                    table.AddRow(listing.Category.ToString(), listing.Title, listing.Origin, listing.Destination,
                        listing.Date.ToString("o"), listing.ListingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Insert(0, "View my listings");
                break;
            }
            case 0:
                AnsiConsole.MarkupLine("No listings found");
                break;
        }

        AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        var table2 = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);

        table2.AddColumn("[bold]Type[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Rating[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Comment[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Date[/]", col => col.LeftAligned());

        List<Review?> reviews = rs.GetReviews(state.currentUUID);
        switch (reviews.Count)
        {
            case > 5:
            {
                foreach (Review? review in reviews.GetRange(0, 5))
                {
                    table2.AddRow(review.Title, review.Type, review.Rating.ToString(),
                        review.Comment, review.CreatedAt.ToString("o"));
                }
                AnsiConsole.Write(table2);
                choices.Insert(0, "View my reviews");
                break;
            }
            case > 0:
            {
                foreach (Review? review in reviews)
                {
                    table2.AddRow(review.Title, review.Type, review.Rating.ToString(),
                        review.Comment, review.CreatedAt.ToString("o"));
                }
                AnsiConsole.Write(table2);
                choices.Insert(0, "View my reviews");
                break;
            }
            case 0:
                AnsiConsole.MarkupLine("No reviews found");
                break;
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhere would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));

        return choice switch
        {
            "View my listings" => "MyListings",
            "Create listing" => "CreateListing",
            "View my reviews" => "MyReviews",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
    }
}
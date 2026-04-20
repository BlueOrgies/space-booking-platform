using System.Data.SQLite;
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
        
        ListingService ls = new ListingService();
        ReviewService rs = new ReviewService();
        var choices = new List<string> {"Create listing", "Go back to main menu", "Quit" };
        
        AnsiConsole.Write(new Rule($"[bold green]{state.CurrentUser}s profile: Organizer[/]").RuleStyle("green"));
        
        double rating = rs.GetAverageRating(state.CurrentUUID);
        if (rating > 0.0)
        {
            AnsiConsole.MarkupLine($"\nAverage rating: [green]{rating}[/]");
        }

        AnsiConsole.MarkupLine("\n[green]My listings[/]");
        var table = new Table()
            .MinimalDoubleHeadBorder()
            .BorderColor(Color.Green);
        
        table.AddColumn("[bold]Category[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Origin[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Destination[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Date[/]", col => col.LeftAligned());
        table.AddColumn("[bold]Status[/]", col => col.LeftAligned());
        
        List<Listings> listings = ls.GetListingsById(state.CurrentUUID, Limit, 0);
        if (listings.Count > 0)
        {
            foreach (Listings listing in listings.GetRange(0, 5))
            {
                table.AddRow(listing.Category.ToString(), listing.Title, listing.Origin, listing.Destination,
                    listing.Date.ToString("o"), listing.ListingStatus.ToString());
            }

            AnsiConsole.Write(table);
            choices.Insert(0, "View my listings");
            state.Offset = 0;
        }
        else
        {
                AnsiConsole.MarkupLine("[grey]No listings available[/]");
        }

        AnsiConsole.MarkupLine("\n[green]My reviews[/]");
        var table2 = new Table()
            .MinimalDoubleHeadBorder()
            .BorderColor(Color.Grey);

        table2.AddColumn("[bold]Type[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Title[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Rating[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Comment[/]", col => col.LeftAligned());
        table2.AddColumn("[bold]Date[/]", col => col.LeftAligned());

        List<Review?> reviews = rs.GetReviews(state.CurrentUUID, Limit);
        if (reviews.Count > 0)
        {
            foreach (Review? review in reviews.GetRange(0, 5))
            {
                table2.AddRow(review.Title, review.Type, review.Rating.ToString(),
                    review.Comment, review.CreatedAt.ToString("o"));
            }

            AnsiConsole.Write(table2);
            choices.Insert(0, "View my reviews");
            state.Offset = 0;
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No reviews available[/]");
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
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
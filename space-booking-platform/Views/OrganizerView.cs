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
                for (int i = 0; i < 5; i++)
                {
                    table.AddRow(listings[i].Category.ToString(), listings[i].Title, listings[i].Origin, listings[i].Destination,
                        listings[i].Date.ToString("o"), listings[i].ListingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Add("View my listings");
                break;
            }
            case > 0:
            {
                for (int i = 0; listings.Count > i; i++)
                {
                    table.AddRow(listings[i].Category.ToString(), listings[i].Title, listings[i].Origin, listings[i].Destination,
                        listings[i].Date.ToString("o"), listings[i].ListingStatus.ToString());
                }
                AnsiConsole.Write(table);
                choices.Add("View my listings");
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
                for (int i = 0; i < 5; i++)
                {
                    table2.AddRow(reviews[i].Title, reviews[i].Type, reviews[i].Rating.ToString(),
                        reviews[i].Comment, reviews[i].CreatedAt.ToString("o"));
                }
                AnsiConsole.Write(table2);
                choices.Add("View my reviews");
                break;
            }
                case > 0:
            {
                for (int i = 0; i < reviews.Count; i++)
                {
                    table2.AddRow(reviews[i].Title, reviews[i].Type, reviews[i].Rating.ToString(),
                        reviews[i].Comment, reviews[i].CreatedAt.ToString("o"));
                }
                AnsiConsole.Write(table2);
                choices.Add("View my reviews");
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
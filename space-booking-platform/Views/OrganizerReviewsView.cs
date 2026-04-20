using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class OrganizerReviewsView(AppState state)
{
    private const int PageSize = 5;

    public string? Display()
    {
        int currentPage = 0;
        ReviewService reviewService = new ReviewService();
        List<Review?> allReviews = reviewService.GetReviews(state.CurrentUUID);

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold green]My Reviews[/]").RuleStyle("green"));

            if (allReviews.Count == 0)
            {
                AnsiConsole.MarkupLine("You have no reviews yet.");
                AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("Go back to profile"));
                return "OrganizerView";
            }

            int startIndex = currentPage * PageSize;
            int endIndex = Math.Min(startIndex + PageSize, allReviews.Count);
            int totalPages = (int)Math.Ceiling((double)allReviews.Count / PageSize);

            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey);

            table.AddColumn("[bold]Listing[/]", col => col.LeftAligned());
            table.AddColumn("[bold]Type[/]", col => col.LeftAligned());
            table.AddColumn("[bold]Rating[/]", col => col.LeftAligned());
            table.AddColumn("[bold]Comment[/]", col => col.LeftAligned());
            table.AddColumn("[bold]Date[/]", col => col.LeftAligned());

            for (int i = startIndex; i < endIndex; i++)
            {
                Review? review = allReviews[i];
                if (review == null) continue;

                string stars = $"[yellow]{new string('★', review.Rating)}[/][grey]{new string('☆', 5 - review.Rating)}[/]";
                table.AddRow(
                    Markup.Escape(review.Title),
                    Markup.Escape(review.Type),
                    stars,
                    Markup.Escape(review.Comment),
                    review.CreatedAt.ToString("yyyy-MM-dd"));
            }

            AnsiConsole.Write(table);

            var prompt = new SelectionPrompt<string>()
                .Title($"\nPage {currentPage + 1} of {totalPages}. Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow));

            if (endIndex < allReviews.Count)
                prompt.AddChoice("Next page");
            if (startIndex > 0)
                prompt.AddChoice("Previous page");

            prompt.AddChoices("Go back to profile", "Go back to main menu", "Quit");

            var choice = AnsiConsole.Prompt(prompt);

            switch (choice)
            {
                case "Next page":
                    currentPage++;
                    break;
                case "Previous page":
                    currentPage--;
                    break;
                case "Go back to profile":
                    return "OrganizerView";
                case "Go back to main menu":
                    return "Home";
                default:
                    return null;
            }
        }
    }
}
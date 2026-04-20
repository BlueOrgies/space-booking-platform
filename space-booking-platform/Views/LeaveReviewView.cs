using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class LeaveReviewView(AppState state)
{
    public string? Display()
    {
        ReviewService reviewService = new ReviewService();
        
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]=== Leave a Review ===[/]\n");
        
        int rating = AnsiConsole.Prompt(
            new TextPrompt<int>("How would you rate your experience (1-6)?")
                .ValidationErrorMessage("[red]Rating must be between 1 and 6[/]")
                .Validate(n => n is >= 1 and <= 6));
        
        string comment = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold]Comment (optional):[/]")
                .AllowEmpty());
        
        reviewService.CreateReview(state.currentUUID, state.currentBookingID, rating, comment);
        
        AnsiConsole.MarkupLine("\n[green]Thank you for your review![/]");
        AnsiConsole.WriteLine("Press any key to return to your bookings...");
        Console.ReadKey(true);
        
        return "MyBookings";
    }
}
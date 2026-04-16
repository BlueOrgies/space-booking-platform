namespace space_booking_platform.Views;

public class LeaveReviewView
{
    public string? Display(AppState state)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]=== Leave a Review ===[/]\n");
        
        int rating = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("How would you rate your experience?")
                .AddChoices(1, 2, 3, 4, 5, 6));
        
        string comment = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold]Comment (optional):[/]")
                .AllowEmpty());
        
        return "";
    }
}
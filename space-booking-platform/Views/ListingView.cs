using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class ListingView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();

        var listingService = new ListingService();
        Listings? listing = listingService.GetListingById(state.currentListingID);

        if (listing == null)
        {
            AnsiConsole.MarkupLine("[red]Listing not found.[/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("Back to Browse Listings"));
            return "BrowseListings";
        }

        AnsiConsole.Write(new Rule($"[bold green]{Markup.Escape(listing.Title)}[/]").RuleStyle("green"));

        var table = new Table().RoundedBorder().BorderColor(Color.Grey).HideHeaders();
        table.AddColumn("");
        table.AddColumn("");

        table.AddRow("[bold]Category[/]",    listing.Category.ToString());
        table.AddRow("[bold]Description[/]", Markup.Escape(listing.Description));
        table.AddRow("[bold]Transport[/]",   Markup.Escape(listing.TransportMethod));
        table.AddRow("[bold]Origin[/]",      Markup.Escape(listing.Origin));
        table.AddRow("[bold]Destination[/]", Markup.Escape(listing.Destination));
        table.AddRow("[bold]Date[/]",        listing.Date.ToString("yyyy-MM-dd HH:mm"));
        table.AddRow("[bold]Duration[/]",    $"{listing.Duration} {listing.DurationType}");
        table.AddRow("[bold]Capacity[/]",    $"{listing.Capacity} {listing.CapacityUnit}");

        var bookingService = new BookingService();
        int booked = bookingService.GetBookingCount(listing.ListingId);
        int remaining = listing.Capacity - booked;
        string availColor = remaining > 0 ? "green" : "red";
        table.AddRow("[bold]Availability[/]", $"[{availColor}]{booked}/{listing.Capacity} booked ({remaining} remaining)[/]");

        string priceDisplay = listing.PriceUnit == ListingPriceUnit.EurosPerKg && state.isLoggedIn && state.currentUserWeight > 0
            ? $"{listing.Price} €/kg (Your total: [bold]{listing.Price * state.currentUserWeight} €[/] for {state.currentUserWeight} kg)"
            : $"{listing.Price} {listing.PriceUnit}";
        table.AddRow("[bold]Price[/]",       priceDisplay);

        string statusColor = listing.ListingStatus switch
        {
            ListingStatus.Active => "green",
            ListingStatus.Cancelled => "red",
            _ => "yellow"
        };
        table.AddRow("[bold]Status[/]",      $"[{statusColor}]{listing.ListingStatus}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var choices = new List<string>();

        if (state.isLoggedIn && listing.UUID == state.currentUUID)
        {
            choices.Add("Edit this listing");
        }
        else if (state.isLoggedIn && listing.ListingStatus == ListingStatus.Active)
        {
            if (bookingService.HasBooked(state.currentUUID, listing.ListingId))
                AnsiConsole.MarkupLine("[grey]You have already booked this listing.[/]");
            else if (remaining <= 0)
                AnsiConsole.MarkupLine("[red]This listing is fully booked.[/]");
            else
                choices.Add("Book this listing");
        }

        choices.Add("Back to Browse Listings");
        choices.Add("Back to main menu");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));

        if (choice == "Book this listing")
        {
            var bookingService = new BookingService();
            bookingService.CreateBooking(state.currentUUID, listing.ListingId);
            AnsiConsole.MarkupLine("[bold green]Booking confirmed![/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey(intercept: true);
            return "BrowseListings";
        }

        if (choice == "Edit this listing")
        {
            state.currentListingID = listing.ListingId;
            return "EditListing";
        }

        return choice switch
        {
            "Back to Browse Listings" => "BrowseListings",
            _                         => "Home"
        };
    }
}

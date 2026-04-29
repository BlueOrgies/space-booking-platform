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
        Listings? listing = listingService.GetListingById(state.CurrentListingID);

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
        table.AddRow("[bold]Title[/]",       Markup.Escape(listing.Title));
        table.AddRow("[bold]Category[/]",    listing.Category.ToString());
        table.AddRow("[bold]Description[/]", Markup.Escape(listing.Description));

        switch (listing)
        {
            case PassengerTransportation pt:
                table.AddRow("[bold]Transport[/]",   Markup.Escape(pt.TransportMethod));
                table.AddRow("[bold]Origin[/]",      Markup.Escape(pt.Origin));
                table.AddRow("[bold]Destination[/]", Markup.Escape(pt.Destination));
                table.AddRow("[bold]Luggage[/]",     pt.LuggageIncluded ? "Included" : "Not included");
                break;
            case FreightHaul fh:
                table.AddRow("[bold]Transport[/]",   Markup.Escape(fh.TransportMethod));
                table.AddRow("[bold]Origin[/]",      Markup.Escape(fh.Origin));
                table.AddRow("[bold]Destination[/]", Markup.Escape(fh.Destination));
                table.AddRow("[bold]Hazardous[/]",   fh.HazardousMaterialsAllowed ? "Allowed" : "Not allowed");
                break;
            case Accommodation acc:
                table.AddRow("[bold]Location[/]",  Markup.Escape(acc.Location));
                table.AddRow("[bold]Pets[/]",      acc.PetsAllowed ? "Allowed" : "Not allowed");
                break;
            case Activity act:
                table.AddRow("[bold]Location[/]",  Markup.Escape(act.Location));
                table.AddRow("[bold]Min Age[/]",   act.MinAge > 0 ? act.MinAge.ToString() : "None");
                break;
        }
        table.AddRow("[bold]Date[/]",        listing.Date.ToString("yyyy-MM-dd HH:mm"));
        table.AddRow("[bold]Duration[/]",    $"{listing.Duration} {listing.DurationType}");
        table.AddRow("[bold]Capacity[/]",    $"{listing.Capacity} {listing.CapacityUnit}");

        var bookingService = new BookingService();
        int booked = bookingService.GetBookingCount(listing.ListingId);
        bool isFull;
        int CurrentUserWeight = state.CurrentUserWeight;

        if (listing.CapacityUnit == ListingCapacityUnit.MaxWeight)
        {
            int bookedWeight = bookingService.GetBookedWeight(listing.ListingId);
            int remainingWeight = listing.Capacity - bookedWeight;
            string wAvailColor = remainingWeight > 0 ? "green" : "red";
            table.AddRow("[bold]Availability[/]", $"[{wAvailColor}]{bookedWeight}/{listing.Capacity} kg used ({remainingWeight} kg remaining)[/]");
            isFull = remainingWeight < CurrentUserWeight;
        }
        else
        {
            int remaining = listing.Capacity - booked;
            string availColor = remaining > 0 ? "green" : "red";
            table.AddRow("[bold]Availability[/]", $"[{availColor}]{booked}/{listing.Capacity} booked ({remaining} remaining)[/]");
            isFull = remaining <= 0;
        }

        string priceDisplay = listing.PriceUnit == ListingPriceUnit.EurosPerKg
            ? $"{listing.Price} €/kg (Your total: [bold]{listing.Price * CurrentUserWeight} €[/] for {CurrentUserWeight} kg)"
            : $"{listing.Price} {listing.PriceUnit}";
        table.AddRow("[bold]Price[/]",       priceDisplay);

        string statusColor = listing.ListingStatus switch
        {
            ListingStatus.Upcoming => "green",
            ListingStatus.Cancelled => "red",
            _ => "yellow"
        };
        table.AddRow("[bold]Status[/]",      $"[{statusColor}]{listing.ListingStatus}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        bool hasBooked = state.IsLoggedIn && bookingService.HasBooked(state.CurrentUUID, listing.ListingId);

        if (hasBooked)
        {
            DateTime? bookedOn = bookingService.GetBookingDate(state.CurrentUUID, listing.ListingId);
            if (bookedOn.HasValue)
                AnsiConsole.MarkupLine($"[grey]You booked this listing on {bookedOn.Value:yyyy-MM-dd HH:mm}.[/]");
            AnsiConsole.WriteLine();
        }

        var choices = new List<string>();
        bool isOrganizer = state.IsLoggedIn && listing.UUID == state.CurrentUUID;

        if (isOrganizer)
        {
            choices.Add("Edit this listing");
            if (listing.ListingStatus == ListingStatus.Upcoming)
                choices.Add("Cancel this listing");
        }
        else if (hasBooked)
        {
            if (listing.ListingStatus == ListingStatus.Upcoming)
                choices.Add("Cancel my booking");

            if (listing.ListingStatus == ListingStatus.Past)
            {
                int? bookingId = bookingService.GetBookingId(state.CurrentUUID, listing.ListingId);
                var reviewService = new ReviewService();
                if (bookingId.HasValue && !reviewService.HasReview(bookingId.Value))
                    choices.Add("Leave a review");
            }
        }
        else if (state.IsLoggedIn && listing.ListingStatus == ListingStatus.Upcoming)
        {
            if (isFull)
                AnsiConsole.MarkupLine(listing.CapacityUnit == ListingCapacityUnit.MaxWeight
                    ? $"[red]Not enough weight capacity (your weight: {state.CurrentUserWeight} kg).[/]"
                    : "[red]This listing is fully booked.[/]");
            else
                choices.Add("Book this listing");
        }

        var navChoices = isOrganizer
            ? new[] { "Back to My Listings", "Back to Browse Listings", "Back to main menu" }
            : new[] { "Back to Browse Listings", "Back to main menu" };

        var prompt = new SelectionPrompt<string>()
            .HighlightStyle(new Style(Color.Yellow));

        if (choices.Count > 0)
            prompt.AddChoiceGroup("", choices);

        prompt.AddChoiceGroup("\n"+"Navigation", navChoices);

        var choice = AnsiConsole.Prompt(prompt);

        if (choice == "Book this listing")
        {
            if (!AnsiConsole.Confirm("Are you sure you want to book this listing?"))
                return "Listing";

            bookingService.CreateBooking(state.CurrentUUID, listing.ListingId);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[bold green]✓ Booking Confirmed![/]").RuleStyle("green"));
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey(intercept: true);
            return "Listing";
        }

        if (choice == "Leave a review")
        {
            int? bookingId = bookingService.GetBookingId(state.CurrentUUID, listing.ListingId);
            if (bookingId.HasValue)
                state.CurrentBookingID = bookingId.Value;
            return "LeaveReview";
        }

        if (choice == "Cancel my booking")
        {
            if (!AnsiConsole.Confirm("Are you sure you want to cancel your booking?"))
                return "Listing";

            bookingService.CancelBooking(state.CurrentUUID, listing.ListingId);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[bold yellow]Booking cancelled.[/]").RuleStyle("yellow"));
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey(intercept: true);
            return "Listing";
        }

        if (choice == "Cancel this listing")
        {
            if (!AnsiConsole.Confirm("Are you sure you want to cancel this listing?"))
                return "Listing";

            listingService.CancelListing(listing.ListingId, state.CurrentUUID);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[bold red]Listing cancelled.[/]").RuleStyle("red"));
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey(intercept: true);
            return "Listing";
        }

        if (choice == "Edit this listing")
        {
            state.CurrentListingID = listing.ListingId;
            return "EditListing";
        }

        return choice switch
        {
            "Back to My Listings"     => "MyListings",
            "Back to Browse Listings" => "BrowseListings",
            _                         => "Home"
        };
    }
}

using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class EditListingView(AppState state)
{
    public string? Display()
    {
        ListingService listingService = new ListingService();
        int listingId = state.CurrentListingID;

        Listings? listing = listingService.GetListingById(listingId);

        List<string> choices = ["Title", "Description", "Date", "Duration", "Capacity", "Price"];

        if (listing is PassengerTransportation or FreightHaul)
            choices.InsertRange(2, ["Transportation method", "Origin", "Destination"]);

        if (listing is Accommodation)
            choices.Add("Location");

        if (listing is Activity)
        {
            choices.Add("Location");
            choices.Add("Minimum age");
        }
        
        switch (listing.ListingStatus)
        {
            case ListingStatus.Upcoming:
                choices.Add("Cancel listing");
                break;
            case ListingStatus.Cancelled:
                choices.Add("Reactivate listing");
                break;
            case ListingStatus.Past:
                Console.WriteLine("You cannot edit a past listing \nPress any key to continue...." );
                Console.ReadKey();
                return "MyListings";
        }
        var edit = Edit(choices);

        foreach (KeyValuePair<string, string> kvp in edit)
        {
            listingService.EditListing(listingId, kvp.Key, kvp.Value);
        }
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Edit more from this listing", "My listings",
                    "Main menu"));

        return choice switch
        {
            "Edit more from this listing" => "EditListing",
            "My listings" => "MyListings",
            "Main menu" => "Home",
            _ => null // Quit
        };
    }

    private Dictionary<string, string> Edit(List<string> choices)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("[bold]What would you like to edit?:[/]")
            .WrapAround()
            .AddChoices(choices);
        string edit = AnsiConsole.Prompt(prompt);
        
        AnsiConsole.Clear();

        ListingService listingService = new ListingService();
        Listings? listing = listingService.GetListingById(state.CurrentListingID);

        Dictionary<string, string> newValues = new Dictionary<string, string>();
    
        switch (edit)
        {
            case "Title":
                AnsiConsole.MarkupLine($"[bold]Title: {listing.Title}[/]");
                newValues.Add("title",
                    AnsiConsole.Ask<string>("New title: "));
                break;

            case "Description":
                AnsiConsole.MarkupLine($"[bold]Description: {listing.Description}[/]");
                newValues.Add("description",
                    AnsiConsole.Ask<string>("New description: "));
                break;

            case "Transportation method":
                string currentTransport = listing is PassengerTransportation pt1 ? pt1.TransportMethod
                    : listing is FreightHaul fh1 ? fh1.TransportMethod : string.Empty;
                AnsiConsole.MarkupLine($"[bold]Transportation method: {currentTransport}[/]");
                newValues.Add("transportMethod", AnsiConsole.Ask<string>("New transportation method: "));
                break;

            case "Origin":
                string currentOrigin = listing is PassengerTransportation pt2 ? pt2.Origin
                    : listing is FreightHaul fh2 ? fh2.Origin : string.Empty;
                AnsiConsole.MarkupLine($"[bold]Origin: {currentOrigin}[/]");
                newValues.Add("origin", AnsiConsole.Ask<string>("New origin: "));
                break;

            case "Destination":
                string currentDestination = listing is PassengerTransportation pt3 ? pt3.Destination
                    : listing is FreightHaul fh3 ? fh3.Destination : string.Empty;
                AnsiConsole.MarkupLine($"[bold]Destination: {currentDestination}[/]");
                newValues.Add("destination", AnsiConsole.Ask<string>("New destination: "));
                break;

            case "Location":
                string currentLocation = listing is Accommodation acc1 ? acc1.Location
                    : listing is Activity act1 ? act1.Location : string.Empty;
                AnsiConsole.MarkupLine($"[bold]Location: {currentLocation}[/]");
                newValues.Add("location", AnsiConsole.Ask<string>("New location: "));
                break;

            case "Minimum age":
                if (listing is Activity act2)
                    AnsiConsole.MarkupLine($"[bold]Minimum age: {act2.MinAge}[/]");
                newValues.Add("minAge", AnsiConsole.Ask<string>("New minimum age: "));
                break;

            case "Date":
                AnsiConsole.MarkupLine($"[bold]Date and time: {listing.Date:yyyy-MM-dd HH:mm}[/]");
                newValues.Add("date",
                    AnsiConsole.Ask<DateTime>("New date and time (yyyy-MM-dd HH:mm): ")
                        .ToString("o"));
                break;
            
            case "Duration":
                AnsiConsole.MarkupLine($"[bold]Duration: {listing.Duration} {listing.DurationType}[/]");
                string durationType = AnsiConsole.Ask<string>("Edit duration type: ");
                newValues.Add("durationType", durationType);
                newValues.Add("duration", AnsiConsole.Ask<string>($"Edit duration ({durationType}): "));
                break;
            
            case "Capacity":
                AnsiConsole.MarkupLine($"[bold]Capacity: {listing.Capacity} {listing.CapacityUnit}[/]");
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Capacity unit:[/]")
                    .AddChoices(Enum.GetNames<ListingCapacityUnit>());
                var capacityUnit = AnsiConsole.Prompt(prompt);
                newValues.Add("capacityUnit", capacityUnit);
                newValues.Add("capacity", AnsiConsole.Ask<string>($"Edit capacity ({capacityUnit}): "));
                break;
            
            case "Price":
                AnsiConsole.MarkupLine($"[bold]Price: {listing.Price} {listing.PriceUnit}[/]");
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Price unit:[/]")
                    .AddChoices(Enum.GetNames<ListingPriceUnit>());
                var priceUnit = AnsiConsole.Prompt(prompt);
                newValues.Add("priceUnit", priceUnit);
                newValues.Add("price", AnsiConsole.Ask<string>($"Edit price ({priceUnit}): "));
                break;

            case "Cancel listing":
                newValues.Add("listingStatus",
                    nameof(ListingStatus.Cancelled));
                break;

            case "Reactivate listing":
                newValues.Add("listingStatus",
                    nameof(ListingStatus.Upcoming));
                break;
        }
        return newValues;
    }
}
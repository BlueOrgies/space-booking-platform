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

        List<string> choices =
        [
            "Title", "Description", "Transportation method", "Origin", "Destination",
            "Date", "Duration", "Capacity", "Price"
        ];
        
        switch (listing.ListingStatus)
        {
            case ListingStatus.Active:
                choices.Add("Cancel listing");
                break;
            case ListingStatus.Cancelled:
                choices.Add("Reactivate listing");
                break;
            case ListingStatus.Past:
            case ListingStatus.Inactive:
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
                AnsiConsole.MarkupLine($"[bold]Transportation metod: {listing.TransportMethod}[/]");
                newValues.Add("transportMethod",
                    AnsiConsole.Ask<string>("Transportation method: "));
                break;

            case "Origin":
                AnsiConsole.MarkupLine($"[bold]Origin: {listing.Origin}[/]");
                newValues.Add("origin",
                    AnsiConsole.Ask<string>("Edit origin: "));
                break;

            case "Destination":
                AnsiConsole.MarkupLine($"[bold]Destination: {listing.Destination}[/]");
                newValues.Add("destination",
                    AnsiConsole.Ask<string>("Edit destination: "));
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
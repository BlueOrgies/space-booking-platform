using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class EditListingView(AppState state)
{
    public string? Display()
    {
        int listingId = state.currentListingID;
        ListingService listingService = new ListingService();
        
        AnsiConsole.Clear();
        
        var prompt = new SelectionPrompt<string>()
            .Title("[bold]What would you like to edit?:[/]")
            .WrapAround()
            .AddChoices("Title", "Description", "Transportation Method", "Origin", "Destination",
                "Date", "Duration", "Capacity", "Price", "Status");
        string edit = AnsiConsole.Prompt(prompt);

        switch (edit)
        {
            case "Title":
                string title = AnsiConsole.Ask<string>("New title: ");
                listingService.EditListing(listingId, "title", title);
                break;
            case "Description":
                string description = AnsiConsole.Ask<string>("New description: ");
                listingService.EditListing(listingId, "description", description);
                break;
            case "Transportation method":
                string transportation = AnsiConsole.Ask<string>("Transportation method: ");
                listingService.EditListing(listingId, "transportationMethod", transportation);
                break;
            case "Origin":
                string origin = AnsiConsole.Ask<string>("Edit origin: ");
                listingService.EditListing(listingId, "origin", origin);
                break;
            case "Destination":
                string destination = AnsiConsole.Ask<string>("Edit destination: ");
                listingService.EditListing(listingId, "destination", destination);
                break;
            case "Date":
                DateTime date = AnsiConsole.Ask<DateTime>("New date and time (yyyy-MM-dd HH:mm): ");
                listingService.EditListing(listingId, "date", date.ToString("o"));
                break;
            case "Duration":
                int duration = AnsiConsole.Ask<int>("Edit duration: ");
                string durationType = AnsiConsole.Ask<string>("Edit duration type: ");
                listingService.EditListing(listingId, "duration", duration.ToString());
                listingService.EditListing(listingId, "durationType", durationType);
                break;
            case "Capacity":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Capacity unit:[/]")
                    .AddChoices(Enum.GetNames<ListingCapacityUnit>());
                var capacityUnit = AnsiConsole.Prompt(prompt);
                int capacity = AnsiConsole.Ask<int>($"Edit duration ({capacityUnit}): ");
                listingService.EditListing(listingId, "capacity", capacity.ToString());
                listingService.EditListing(listingId, "capacityUnit", capacityUnit);
                break;
            case "Price":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Price unit:[/]")
                    .AddChoices(Enum.GetNames<ListingPriceUnit>());
                var priceUnit = AnsiConsole.Prompt(prompt);
                int price = AnsiConsole.Ask<int>($"Edit duration ({priceUnit}): ");
                listingService.EditListing(listingId, "capacity", price.ToString());
                listingService.EditListing(listingId, "capacityUnit", priceUnit);
                break;
            case "Status":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Price unit:[/]")
                    .AddChoices(Enum.GetNames<ListingStatus>());
                var status = AnsiConsole.Prompt(prompt);
                listingService.EditListing(listingId, "Status", status);
                break;
        }
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Edit more from this listing", "Go back to my listings", "Go back to profile",
                    "Go back to main menu", "Quit"));

        return choice switch
        {
            "Edit more from this listing" => "EditListing",
            "Go back to my listings" => "MyListings",
            "Go back to profile" => "Profile",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
    }
}
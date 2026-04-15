using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class EditListingView(AppState state)
{
    public string? Display()
    {
        int listingId = state.currentListingID;
        ListingService listingService = new ListingService(state);
        
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
                listingService.EditListingInDb("title", title, listingId);
                break;
            case "Description":
                string description = AnsiConsole.Ask<string>("New description: ");
                listingService.EditListingInDb("description", description, listingId);
                break;
            case "Transportation method":
                string transportation = AnsiConsole.Ask<string>("Transportation method: ");
                listingService.EditListingInDb("transportationMethod", transportation, listingId);
                break;
            case "Origin":
                string origin = AnsiConsole.Ask<string>("Edit origin: ");
                listingService.EditListingInDb("origin", origin, listingId);
                break;
            case "Destination":
                string destination = AnsiConsole.Ask<string>("Edit destination: ");
                listingService.EditListingInDb("destination", destination, listingId);
                break;
            case "Date":
                DateTime date = AnsiConsole.Ask<DateTime>("New date and time (yyyy-MM-dd HH:mm): ");
                listingService.EditListingInDb("date", date.ToString(), listingId);
                break;
            case "Duration":
                int duration = AnsiConsole.Ask<int>("Edit duration: ");
                string durationType = AnsiConsole.Ask<string>("Edit duration type: ");
                listingService.EditListingInDb("duration", duration.ToString(), listingId);
                listingService.EditListingInDb("durationType", durationType, listingId);
                break;
            case "Capacity":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Capacity unit:[/]")
                    .AddChoices(Enum.GetNames<ListingCapacityUnit>());
                var capacityUnit = AnsiConsole.Prompt(prompt);
                int capacity = AnsiConsole.Ask<int>($"Edit duration ({capacityUnit}): ");
                listingService.EditListingInDb("capacity", capacity.ToString(), listingId);
                listingService.EditListingInDb("capacityUnit", capacityUnit, listingId);
                break;
            case "Price":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Price unit:[/]")
                    .AddChoices(Enum.GetNames<ListingPriceUnit>());
                var priceUnit = AnsiConsole.Prompt(prompt);
                int price = AnsiConsole.Ask<int>($"Edit duration ({priceUnit}): ");
                listingService.EditListingInDb("capacity", price.ToString(), listingId);
                listingService.EditListingInDb("capacityUnit", priceUnit, listingId);
                break;
            case "Status":
                prompt = new SelectionPrompt<string>()
                    .Title("[bold]Price unit:[/]")
                    .AddChoices(Enum.GetNames<ListingStatus>());
                var status = AnsiConsole.Prompt(prompt);
                listingService.EditListingInDb("Status", status, listingId);
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
            "Edit more from this listing" => "EditListingView",
            "Go back to my listings" => "MyListingsView",
            "Go back to profile" => "OrganizerView",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
        //TODO: Add generic method for this?
    }
}
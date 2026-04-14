using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class CreateListingView(AppState state)
{
    public string? Display()
    {
        if (!state.isOrganizer)
        {
            AnsiConsole.MarkupLine("[bold underline]You are not an organizer and cannot create a listing[/]");
        }
        AnsiConsole.MarkupLine("[bold green]=== Create a listing ===[/]\n");

        ListingService listingService = new ListingService(state);

        int uuid = state.currentUUID;
        
        var prompt = new SelectionPrompt<string>() //Todo: Fix space between title and choices?
            .Title("[bold]Category:[/]")
            .AddChoices(Enum.GetNames<ListingCategory>());
        string category = AnsiConsole.Prompt(prompt);
        ListingCategory categoryEnum = (ListingCategory)Enum.Parse(typeof(ListingCategory), category);
        AnsiConsole.MarkupLine($"[bold]Category: [/]{categoryEnum}");

        string title = AnsiConsole.Ask<string>("[bold]Title: [/]");
        
        string description = AnsiConsole.Ask<string>("[bold]Description: [/]");
        
        string transportMethod = AnsiConsole.Ask<string>("[bold]Transport method: [/]");
        
        string origin = AnsiConsole.Ask<string>("[bold]Origin: [/]");
        
        string destination = AnsiConsole.Ask<string>("[bold]Destination: [/]");
        
        DateTime date = AnsiConsole.Ask<DateTime>("[bold]Date: [/]");
        
        int duration = AnsiConsole.Ask<int>("[bold]Duration: [/]");
        
        string durationType = AnsiConsole.Ask<string>("[bold]Duration type: [/]");
        
        prompt = new SelectionPrompt<string>()
            .Title("[bold]Capacity unit:[/]")
            .AddChoices(Enum.GetNames<ListingCapacityUnit>());
        var capacityUnit = AnsiConsole.Prompt(prompt);
        ListingCapacityUnit capacityUnitEnum = (ListingCapacityUnit)Enum.Parse(typeof(ListingCapacityUnit), capacityUnit);
        
        int capacity = AnsiConsole.Ask<int>($"[bold]Capacity[/] ({capacityUnitEnum}): ");
        
        prompt = new SelectionPrompt<string>()
            .Title("[bold]Price unit:[/]")
            .AddChoices(Enum.GetNames<ListingPriceUnit>());
        var priceUnit = AnsiConsole.Prompt(prompt);
        ListingPriceUnit priceUnitEnum =  (ListingPriceUnit)Enum.Parse(typeof(ListingPriceUnit), priceUnit);
        
        decimal price = AnsiConsole.Ask<decimal>($"[bold]Price[/] ({priceUnitEnum}): ");

        Listings listing = new Listings(uuid, categoryEnum, title, description, transportMethod, origin, destination, date,
           duration, durationType, capacity, capacityUnitEnum, price, priceUnitEnum);
        
        listingService.CreateListing(listing);
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Create another listing", "Go to my listings", "Go to profile",
                    "Go to main menu", "Quit"));

        return choice switch
        {
            "Create another listing" => "CreateListingView",
            "Go back to my listings" => "MyListingsView",
            "Go back to profile" => "OrganizerView",
            "Go back to main menu" => "HomeView",
            _ => null // Quit
        };
    }
}
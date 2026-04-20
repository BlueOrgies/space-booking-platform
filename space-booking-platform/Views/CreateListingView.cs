using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class CreateListingView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold green]=== Create a listing ===[/]\n");

        ListingService listingService = new ListingService();

        int uuid = state.CurrentUUID;
        
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
        
        DateTime date = AnsiConsole.Ask<DateTime>("[bold]Date and time[/] (yyyy-mm-dd hh:mm) : ");
        
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

        listingService.CreateListing(uuid, categoryEnum, title, description, transportMethod, origin, destination,
            date, duration, durationType, capacity, capacityUnitEnum, price, priceUnitEnum, DateTime.Now, ListingStatus.Active);
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Where would you like to go?")
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices("Create another listing", "My listings", "My organizer profile",
                    "My profile", "Go to main menu", "Quit"));

        return choice switch
        {
            "Create another listing" => "CreateListing",
            "My listings" => "MyListings",
            "My organizer profile" => "OrganizerView",
            "My profile" => "ProfileView",
            "Go back to main menu" => "Home",
            _ => null // Quit
        };
    }
}
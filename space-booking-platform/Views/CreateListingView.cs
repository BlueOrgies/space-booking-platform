using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class CreateListingView
{
    public static void CreateListing()
    {
        AnsiConsole.MarkupLine("[bold green]=== Create a listing ===[/]\n");
        
        var prompt = new SelectionPrompt<string>() //The prompts look the same but this has an extra space between title and choices 
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

        Listings listing = new Listings(categoryEnum, title, description, transportMethod, origin, destination, date,
           duration, durationType, capacity, capacityUnitEnum, price, priceUnitEnum);
        
        ListingService.AddListingToTable(listing);
    }
}
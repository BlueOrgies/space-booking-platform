using space_booking_platform.Models;
using space_booking_platform.Services;
using Spectre.Console;

namespace space_booking_platform.Views;

public class CreateListingView(AppState state)
{
    public string? Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold green]Create a listing[/]").RuleStyle("green"));

        ListingService listingService = new ListingService();

        int uuid = state.CurrentUUID;
        
        var prompt = new SelectionPrompt<string>()
            .Title("[bold]Category:[/]")
            .AddChoices(Enum.GetNames<ListingCategory>());
        prompt = prompt.UseConverter(categoryName =>
            categoryName == nameof(ListingCategory.PassengerTransportation) ? "Passenger transportation" :
            categoryName == nameof(ListingCategory.FreightHaul) ? "Freight haul" :
            categoryName);
        string category = AnsiConsole.Prompt(prompt);
        ListingCategory categoryEnum = (ListingCategory)Enum.Parse(typeof(ListingCategory), category);
        AnsiConsole.MarkupLine($"[bold]Category: [/]{categoryEnum.ToDescriptionString()}");

        string title = AnsiConsole.Ask<string>("[bold]Title: [/]");
        
        string description = AnsiConsole.Ask<string>("[bold]Description: [/]");

        string transportMethod = string.Empty;
        string origin = string.Empty;
        string destination = string.Empty;
        string location = string.Empty;
        bool petsAllowed = false;
        bool luggageIncluded = false;
        bool hazardousMaterialsAllowed = false;
        int minAge = 0;

        switch (categoryEnum)
        {
            case ListingCategory.PassengerTransportation:
                transportMethod = AnsiConsole.Ask<string>("[bold]Transport method: [/]");
                origin = AnsiConsole.Ask<string>("[bold]Origin: [/]");
                destination = AnsiConsole.Ask<string>("[bold]Destination: [/]");
                luggageIncluded = AnsiConsole.Confirm("[bold]Luggage included?[/]");
                break;
            case ListingCategory.FreightHaul:
                transportMethod = AnsiConsole.Ask<string>("[bold]Transport method: [/]");
                origin = AnsiConsole.Ask<string>("[bold]Origin: [/]");
                destination = AnsiConsole.Ask<string>("[bold]Destination: [/]");
                hazardousMaterialsAllowed = AnsiConsole.Confirm("[bold]Hazardous materials allowed?[/]");
                break;
            case ListingCategory.Accommodation:
                location = AnsiConsole.Ask<string>("[bold]Location: [/]");
                //TODO: Check this
                petsAllowed = AnsiConsole.Confirm("[bold]Pets allowed?[/]");
                break;
            case ListingCategory.Activity:
                location = AnsiConsole.Ask<string>("[bold]Location: [/]");
                minAge = AnsiConsole.Ask<int>("[bold]Minimum age: [/]");
                break;
        }

        DateTime date = AnsiConsole.Ask<DateTime>("[bold]Date and time[/] (yyyy-mm-dd hh:mm) : ");
        
        int duration = AnsiConsole.Ask<int>("[bold]Duration: [/]");
        
        string durationType = AnsiConsole.Ask<string>("[bold]Duration type: [/]");
        
        prompt = new SelectionPrompt<string>()
            .Title("[bold]Capacity unit:[/]")
            .AddChoices(Enum.GetNames<ListingCapacityUnit>());
        prompt = prompt.UseConverter(categoryName =>
            categoryName == nameof(ListingCapacityUnit.MaxWeight) ? "Max weight" : categoryName);
        var capacityUnit = AnsiConsole.Prompt(prompt);
        ListingCapacityUnit capacityUnitEnum = (ListingCapacityUnit)Enum.Parse(typeof(ListingCapacityUnit), capacityUnit);
        
        int capacity = AnsiConsole.Ask<int>($"[bold]Capacity[/] ({capacityUnitEnum.ToDescriptionString()}): ");
        
        prompt = new SelectionPrompt<string>()
            .Title("[bold]Price unit:[/]")
            .AddChoices(Enum.GetNames<ListingPriceUnit>());
        prompt = prompt.UseConverter(categoryName =>
            categoryName == nameof(ListingPriceUnit.EurosPerKg) ? "Euros/kg" : categoryName);
        var priceUnit = AnsiConsole.Prompt(prompt);
        ListingPriceUnit priceUnitEnum =  (ListingPriceUnit)Enum.Parse(typeof(ListingPriceUnit), priceUnit);
        
        decimal price = AnsiConsole.Ask<decimal>($"[bold]Price[/] ({priceUnitEnum.ToDescriptionString()}): ");

        listingService.CreateListing(uuid, categoryEnum, title, description, transportMethod, origin, destination,
            date, duration, durationType, capacity, capacityUnitEnum, price, priceUnitEnum, DateTime.Now, ListingStatus.Upcoming,
            location, petsAllowed, luggageIncluded, hazardousMaterialsAllowed, minAge);
        
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
namespace space_booking_platform.Models;

public class Listings
{
    private ListingCategory _category;

    private string _title;

    private string _description;

    private string _transportMethod;

    private string _origin;

    private string _destination;

    private DateTime _date;

    private int _duration;

    private string _durationType;

    private int _capacity;

    private ListingCapacityUnit _capacityUnit;

    private decimal _price;

    private ListingPriceUnit _priceUnit;

    private DateTime _createdAt;

    private ListingStatus _listingStatus;

    public ListingCategory Category
    {
        get => _category;
        set => _category = value;
    }

    public string Title
    {
        get => _title;
        set => _title = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public string TransportMethod
    {
        get => _transportMethod;
        set => _transportMethod = value;
    }

    public string Origin
    {
        get => _origin;
        set => _origin = value;
    }

    public string Destination
    {
        get => _destination;
        set => _destination = value;
    }

    public DateTime Date
    {
        get => _date;
        set => _date = value;
    }

    public int Duration
    {
        get => _duration;
        set => _duration = value;
    }

    public string DurationType
    {
        get => _durationType;
        set => _durationType = value;
    }

    public int Capacity
    {
        get => _capacity;
        set => _capacity = value;
    }

    public ListingCapacityUnit CapacityUnit
    {
        get => _capacityUnit;
        set => _capacityUnit = value;
    }

    public decimal Price
    {
        get => _price;
        set => _price = value;
    }

    public ListingPriceUnit PriceUnit
    {
        get => _priceUnit;
        set => _priceUnit = value;
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = value;
    }

    public ListingStatus ListingStatus
    {
        get => _listingStatus;
        set => _listingStatus = value;
    }

    public Listings(ListingCategory category, string title, string description, 
        string transportMethod, string origin, string destination, DateTime date, int duration,
        string durationType, int capacity, ListingCapacityUnit capacityUnit, decimal price, 
        ListingPriceUnit priceUnit)
    {
        _category = category;
        _title = title;
        _description = description;
        _transportMethod = transportMethod;
        _origin = origin;
        _destination = destination;
        _date = date;
        _duration = duration;
        _durationType = durationType;
        _capacity = capacity;
        _capacityUnit = capacityUnit;
        _price = price;
        _priceUnit = priceUnit;
        _createdAt = DateTime.Now;
        _listingStatus = ListingStatus.Active;
    }
}
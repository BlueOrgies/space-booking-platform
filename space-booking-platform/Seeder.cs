using System.Data.SQLite;
using space_booking_platform.Models;
using space_booking_platform.Services;

namespace space_booking_platform;

public static class Seeder
{
    public static void SeedIfEmpty()
    {
        using SQLiteConnection conn = Database.ConnectToDb();

        using SQLiteCommand countCmd = new SQLiteCommand("SELECT COUNT(*) FROM users", conn);
        long userCount = (long)countCmd.ExecuteScalar()!;
        if (userCount > 0) return;

        var userService = new UserService();

        // --- Users ---
        string[][] users =
        [
            ["alice",   "pass1", "60", "165", "1"],
            ["bob",     "pass2", "80", "180", "1"],
            ["carol",   "pass3", "55", "160", "1"],
            ["dave",    "pass4", "90", "185", "1"],
            ["eve",     "pass5", "65", "170", "1"],
            ["frank",   "pass6", "75", "175", "0"],
            ["grace",   "pass7", "58", "162", "0"],
            ["henry",   "pass8", "85", "178", "0"],
            ["iris",    "pass9", "52", "158", "0"],
            ["jack",   "pass10", "95", "190", "0"],
            ["karen",  "pass11", "62", "168", "1"],
            ["leo",    "pass12", "78", "182", "1"],
            ["mia",    "pass13", "57", "163", "1"],
            ["noah",   "pass14", "88", "177", "1"],
            ["olivia", "pass15", "61", "166", "0"],
            ["peter",  "pass16", "73", "174", "0"],
            ["quinn",  "pass17", "54", "159", "0"],
            ["rosa",   "pass18", "66", "171", "0"],
            ["sam",    "pass19", "82", "183", "0"],
            ["tina",   "pass20", "59", "161", "0"],
        ];

        var createdUsers = new List<User>();
        foreach (var u in users)
        {
            var user = userService.Register(u[0], u[1], int.Parse(u[2]), int.Parse(u[3]), u[4] == "1");
            createdUsers.Add(user);
        }

        // --- Listings ---
        (string title, string desc, string transport, string origin, string dest,
         int daysOffset, ListingCategory cat, int capacity, ListingCapacityUnit capUnit,
         decimal price, ListingPriceUnit priceUnit)[] listingData =
        [
            ("Lunar Shuttle Transfer",    "Comfortable shuttle from Earth orbit to Luna Base.",     "Shuttle",    "Earth Orbit",    "Luna Base",       30,  ListingCategory.Transportation, 40,  ListingCapacityUnit.Seats,     250m,  ListingPriceUnit.Euros),
            ("Mars Colony Berth",         "Economy berth on a 7-month transit to Mars.",            "Freighter",  "Earth",          "Mars Colony",     90,  ListingCategory.Accommodation,  12,  ListingCapacityUnit.Beds,      4500m, ListingPriceUnit.Euros),
            ("Asteroid Mining Tour",      "Guided tour of a working asteroid mining operation.",    "Shuttle",    "Ceres Station",  "Belt Sector 7",   15,  ListingCategory.Activity,       20,  ListingCapacityUnit.Seats,     320m,  ListingPriceUnit.Euros),
            ("Europa Ice Dive",           "Submersible dive beneath the ice shelf of Europa.",      "Submarine",  "Europa Base",    "Ocean Vent 3",    60,  ListingCategory.Activity,       8,   ListingCapacityUnit.Seats,     890m,  ListingPriceUnit.Euros),
            ("Titan Cargo Express",       "Cargo haul with passenger space available.",             "Tanker",     "Saturn Orbit",   "Titan Harbor",    45,  ListingCategory.Transportation, 6,   ListingCapacityUnit.MaxWeight, 1800m, ListingPriceUnit.EurosPerKg),
            ("Orbital Hotel Suite",       "Luxury suite aboard the Helios Orbital Hotel.",         "Ferry",      "Earth",          "Helios Station",  10,  ListingCategory.Accommodation,  2,   ListingCapacityUnit.Beds,      1200m, ListingPriceUnit.Euros),
            ("Venus Atmosphere Balloon",  "48-hour balloon drift through the Venus cloud layer.",   "Balloon",    "Venus Orbit",    "Cloud City",      20,  ListingCategory.Activity,       12,  ListingCapacityUnit.Seats,     750m,  ListingPriceUnit.Euros),
            ("Deep Space Freighter Hop",  "Affordable hop aboard a deep-space freight run.",        "Freighter",  "Jupiter Station","Uranus Outpost",  180, ListingCategory.Transportation, 4,   ListingCapacityUnit.Seats,     3200m, ListingPriceUnit.Euros),
            ("Phobos Hostel Bunk",        "Budget bunk in Phobos transit hostel.",                  "Ferry",      "Mars Orbit",     "Phobos",          5,   ListingCategory.Accommodation,  30,  ListingCapacityUnit.Beds,      85m,   ListingPriceUnit.Euros),
            ("Zero-G Sports Arena",       "Full-day access to a zero-gravity sports complex.",      "Ferry",      "Earth",          "Apex Station",    7,   ListingCategory.Activity,       50,  ListingCapacityUnit.Seats,     190m,  ListingPriceUnit.Euros),
            ("Saturn Ring Overflight",    "Scenic low-altitude pass over the rings of Saturn.",     "Scout Ship", "Saturn Station", "Ring Sector A",   14,  ListingCategory.Activity,       10,  ListingCapacityUnit.Seats,     620m,  ListingPriceUnit.Euros),
            ("Io Geothermal Stay",        "Research station accommodation near active volcanoes.",  "Shuttle",    "Jupiter Orbit",  "Io Base Alpha",   30,  ListingCategory.Accommodation,  8,   ListingCapacityUnit.Beds,      540m,  ListingPriceUnit.Euros),
            ("High-Speed Transit Pod",    "Point-to-point high-speed pod between Mars cities.",     "Pod",        "Olympus City",   "Hellas Port",     2,   ListingCategory.Transportation, 1,   ListingCapacityUnit.Seats,     45m,   ListingPriceUnit.Euros),
            ("Ganymede Research Trip",    "Join a 2-week research expedition on Ganymede.",         "Research Vessel", "Jupiter Station", "Ganymede Lab", 120, ListingCategory.Activity,  16,  ListingCapacityUnit.Seats,     2100m, ListingPriceUnit.Euros),
            ("Neptune Explorer Passage",  "Rare passenger berth on a Neptune explorer vessel.",     "Explorer",   "Uranus Outpost", "Neptune Orbit",   270, ListingCategory.Transportation, 3,   ListingCapacityUnit.Seats,     8500m, ListingPriceUnit.Euros),
            ("Callisto Cabin Retreat",    "Private off-grid cabin on Callisto's frozen plains.",    "Shuttle",    "Jupiter Orbit",  "Callisto Base",   40,  ListingCategory.Accommodation,  4,   ListingCapacityUnit.Beds,      670m,  ListingPriceUnit.Euros),
            ("Ceres Marketplace Tour",    "Guided cultural tour through Ceres central market.",     "Shuttle",    "Belt Waypoint",  "Ceres Station",   8,   ListingCategory.Activity,       25,  ListingCapacityUnit.Seats,     110m,  ListingPriceUnit.Euros),
            ("Lunar Surface Rover Hire",  "Self-drive rover hire across the lunar highlands.",      "Rover",      "Luna Base",      "Highlands Zone",  3,   ListingCategory.Activity,       2,   ListingCapacityUnit.Seats,     340m,  ListingPriceUnit.Euros),
            ("Trojan Station Shuttle",    "Weekly shuttle service to the Jupiter Trojan stations.", "Shuttle",    "Jupiter Station","Trojan L4",       21,  ListingCategory.Transportation, 20,  ListingCapacityUnit.Seats,     980m,  ListingPriceUnit.Euros),
            ("Enceladus Geyser Viewing",  "Front-row seat to Enceladus' famous water geysers.",    "Scout Ship", "Saturn Station", "Enceladus South", 35,  ListingCategory.Activity,       6,   ListingCapacityUnit.Seats,     430m,  ListingPriceUnit.Euros),
        ];

        var listingService = new ListingService();
        var rng = new Random(42);
        var organizers = createdUsers.Where(u => u.IsOrganizer).ToList();
        int i = 0;

        foreach (var (title, desc, transport, origin, dest, days, cat, capacity, capUnit, price, priceUnit) in listingData)
        {
            var owner = organizers[i % organizers.Count];
            listingService.CreateListing(
                uuid: owner.UserId,
                category: cat,
                title: title,
                description: desc,
                transportMethod: transport,
                origin: origin,
                destination: dest,
                date: DateTime.UtcNow.AddDays(days + rng.Next(-3, 4)),
                duration: rng.Next(1, 14),
                durationType: "Days",
                capacity: capacity,
                capacityUnit: capUnit,
                price: price,
                priceUnit: priceUnit,
                createdAt: DateTime.UtcNow,
                listingStatus: ListingStatus.Upcoming
            );
            i++;
        }
    }
}

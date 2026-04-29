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
         decimal price, ListingPriceUnit priceUnit, ListingStatus status,
         string location, bool petsAllowed, bool luggageIncluded, bool hazardousMaterialsAllowed, int minAge)[] listingData =
        [
            ("Lunar Shuttle Transfer", "Comfortable shuttle from Earth orbit to Luna Base.", "Shuttle", "Earth Orbit", "Luna Base", 30, ListingCategory.PassengerTransportation, 40, ListingCapacityUnit.Seats, 250m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "", false, true, false, 0),
            ("Mars Colony Berth", "Economy berth on a 7-month transit to Mars.", "Freighter", "Earth", "Mars Colony", 90, ListingCategory.Accommodation, 12, ListingCapacityUnit.Beds, 4500m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Mars Colony Habitat C", false, false, false, 0),
            ("Asteroid Mining Tour", "Guided tour of a working asteroid mining operation.", "Shuttle", "Ceres Station", "Belt Sector 7", 15, ListingCategory.Activity, 20, ListingCapacityUnit.Seats, 320m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Ceres Station", false, false, false, 16),
            ("Europa Ice Dive", "Submersible dive beneath the ice shelf of Europa.", "Submarine", "Europa Base", "Ocean Vent 3", 60, ListingCategory.Activity, 8, ListingCapacityUnit.Seats, 890m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Europa Base", false, false, false, 18),
            ("Titan Cargo Express", "Cargo haul with passenger space available.", "Tanker", "Saturn Orbit", "Titan Harbor", 45, ListingCategory.PassengerTransportation, 2500, ListingCapacityUnit.MaxWeight, 1800m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, false, 0),
            ("Orbital Hotel Suite", "Luxury suite aboard the Helios Orbital Hotel.", "Ferry", "Earth", "Helios Station", 10, ListingCategory.Accommodation, 2, ListingCapacityUnit.Beds, 1200m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Helios Station", true, false, false, 0),
            ("Venus Atmosphere Balloon", "48-hour balloon drift through the Venus cloud layer.", "Balloon", "Venus Orbit", "Cloud City", 20, ListingCategory.Activity, 12, ListingCapacityUnit.Seats, 750m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Cloud City", false, false, false, 12),
            ("Deep Space Freight Haul", "Commercial freight run with optional parcel handling.", "Freighter", "Jupiter Station", "Uranus Outpost", 18, ListingCategory.FreightHaul, 5000, ListingCapacityUnit.MaxWeight, 120m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, true, 0),
            ("Phobos Hostel Bunk", "Budget bunk in Phobos transit hostel.", "Ferry", "Mars Orbit", "Phobos", -3, ListingCategory.Accommodation, 30, ListingCapacityUnit.Beds, 85m, ListingPriceUnit.Euros, ListingStatus.Past, "Phobos Hostel", false, false, false, 0),
            ("Zero-G Sports Arena", "Full-day access to a zero-gravity sports complex.", "Ferry", "Earth", "Apex Station", -1, ListingCategory.Activity, 50, ListingCapacityUnit.Seats, 190m, ListingPriceUnit.Euros, ListingStatus.Past, "Apex Station", false, false, false, 10),
            ("Saturn Ring Overflight", "Scenic low-altitude pass over the rings of Saturn.", "Scout Ship", "Saturn Station", "Ring Sector A", 14, ListingCategory.Activity, 10, ListingCapacityUnit.Seats, 620m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Ring Sector A", false, false, false, 8),
            ("Io Geothermal Stay", "Research station accommodation near active volcanoes.", "Shuttle", "Jupiter Orbit", "Io Base Alpha", 30, ListingCategory.Accommodation, 8, ListingCapacityUnit.Beds, 540m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Io Base Alpha", false, false, false, 0),
            ("High-Speed Transit Pod", "Point-to-point high-speed pod between Mars cities.", "Pod", "Olympus City", "Hellas Port", 2, ListingCategory.PassengerTransportation, 1, ListingCapacityUnit.Seats, 45m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "", false, false, false, 0),
            ("Ganymede Research Trip", "Join a 2-week research expedition on Ganymede.", "Research Vessel", "Jupiter Station", "Ganymede Lab", 120, ListingCategory.Activity, 16, ListingCapacityUnit.Seats, 2100m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Ganymede Lab", false, false, false, 14),
            ("Neptune Explorer Passage", "Rare passenger berth on a Neptune explorer vessel.", "Explorer", "Uranus Outpost", "Neptune Orbit", 270, ListingCategory.PassengerTransportation, 3, ListingCapacityUnit.Seats, 8500m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "", false, false, false, 0),
            ("Callisto Cabin Retreat", "Private off-grid cabin on Callisto's frozen plains.", "Shuttle", "Jupiter Orbit", "Callisto Base", 40, ListingCategory.Accommodation, 4, ListingCapacityUnit.Beds, 670m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Callisto Base", true, false, false, 0),
            ("Ceres Marketplace Tour", "Guided cultural tour through Ceres central market.", "Shuttle", "Belt Waypoint", "Ceres Station", 8, ListingCategory.Activity, 25, ListingCapacityUnit.Seats, 110m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Ceres Station", false, false, false, 0),
            ("Lunar Surface Rover Hire", "Self-drive rover hire across the lunar highlands.", "Rover", "Luna Base", "Highlands Zone", 3, ListingCategory.Activity, 2, ListingCapacityUnit.Seats, 340m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Lunar Highlands", false, false, false, 21),
            ("Trojan Station Shuttle", "Weekly shuttle service to the Jupiter Trojan stations.", "Shuttle", "Jupiter Station", "Trojan L4", 21, ListingCategory.PassengerTransportation, 20, ListingCapacityUnit.Seats, 980m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "", false, true, false, 0),
            ("Enceladus Geyser Viewing", "Front-row seat to Enceladus' famous water geysers.", "Scout Ship", "Saturn Station", "Enceladus South", 35, ListingCategory.Activity, 6, ListingCapacityUnit.Seats, 430m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Enceladus South", false, false, false, 12),
            ("Mercury Supply Run", "Hazmat-certified delivery lane to Mercury observatories.", "Industrial Tug", "Venus Transit Hub", "Mercury Terminator", 11, ListingCategory.FreightHaul, 3200, ListingCapacityUnit.MaxWeight, 150m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, true, 0),
            ("Old Earth Museum Crawl", "Curated three-stop museum crawl on Old Earth arcologies.", "Maglev", "Arcology 1", "Arcology 3", -5, ListingCategory.Activity, 18, ListingCapacityUnit.Seats, 95m, ListingPriceUnit.Euros, ListingStatus.Past, "Old Earth Arcologies", false, false, false, 0),
            ("Jovian Monorail Special", "Seasonal monorail package around the Jovian orbital ring.", "Monorail", "Jupiter Ring A", "Jupiter Ring D", 26, ListingCategory.PassengerTransportation, 28, ListingCapacityUnit.Seats, 410m, ListingPriceUnit.Euros, ListingStatus.Cancelled, "", false, false, false, 0),
            ("Kepler Freight Relay", "Long-haul relay for scientific equipment and sealed cargo.", "Freighter", "L2 Depot", "Kepler Transfer Gate", 70, ListingCategory.FreightHaul, 8000, ListingCapacityUnit.MaxWeight, 88m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, true, 0),
            ("Blacksite Wargame Sim", "18+ tactical infiltration simulation in a decommissioned stealth outpost.", "Dropship", "Luna Darkside", "Blacksite-9", 16, ListingCategory.Activity, 14, ListingCapacityUnit.Seats, 540m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Blacksite-9", false, false, false, 18),
            ("Neon Underdeck Extraction", "18+ covert extraction roleplay in the station underdecks with live mission handlers.", "Ghost Van", "Apex Station", "Underdeck Sector K", 9, ListingCategory.Activity, 10, ListingCapacityUnit.Seats, 690m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Underdeck Sector K", false, false, false, 18),
            ("Cipher Drop Smuggling Lane", "Discrete high-risk freight corridor for deniable cargo transfers.", "Cloak Freighter", "Ceres Blind Port", "Titan Shadow Dock", 12, ListingCategory.FreightHaul, 2200, ListingCapacityUnit.MaxWeight, 210m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, true, 0),
            ("Dead-Zone Courier Window", "Tight-timing freight hop through sensor dead zones for sensitive consignments.", "Silent Cutter", "Mars Relay 3", "Europa Fringe", 6, ListingCategory.FreightHaul, 1600, ListingCapacityUnit.MaxWeight, 240m, ListingPriceUnit.EurosPerKg, ListingStatus.Upcoming, "", false, false, true, 0),
            ("Obsidian Casino Sting", "18+ undercover social-engineering operation inside a high-orbit casino ring.", "Stealth Shuttle", "Helios Station", "Obsidian Ring", 27, ListingCategory.Activity, 22, ListingCapacityUnit.Seats, 880m, ListingPriceUnit.Euros, ListingStatus.Upcoming, "Obsidian Ring", false, false, false, 18)
        ];

        var listingService = new ListingService();
        var bookingService = new BookingService();
        var reviewService = new ReviewService();
        var rng = new Random(42);
        var organizers = createdUsers.Where(u => u.IsOrganizer).ToList();
        var travelers = createdUsers.Where(u => !u.IsOrganizer).ToList();
        var createdListings = new List<Listings>();

        int i = 0;
        foreach (var (title, desc, transport, origin, dest, days, cat, capacity, capUnit, price, priceUnit, status, location, petsAllowed, luggageIncluded, hazardousMaterialsAllowed, minAge) in listingData)
        {
            var owner = organizers[i % organizers.Count];
            var created = listingService.CreateListing(
                uuid: owner.UserId,
                category: cat,
                title: title,
                description: desc,
                transportMethod: transport,
                origin: origin,
                destination: dest,
                date: DateTime.UtcNow.AddDays(days + rng.Next(-2, 3)),
                duration: rng.Next(1, 14),
                durationType: "Days",
                capacity: capacity,
                capacityUnit: capUnit,
                price: price,
                priceUnit: priceUnit,
                createdAt: DateTime.UtcNow.AddDays(-rng.Next(1, 20)),
                listingStatus: status,
                location: location,
                petsAllowed: petsAllowed,
                luggageIncluded: luggageIncluded,
                hazardousMaterialsAllowed: hazardousMaterialsAllowed,
                minAge: minAge
            );

            createdListings.Add(created);
            i++;
        }

        // --- Seed bookings across upcoming and past listings ---
        var seededBookingIds = new List<(int BookingId, int UserId, ListingStatus ListingStatus)>();
        int travelerIndex = 0;

        foreach (var seededListing in createdListings.Where(l => l.ListingStatus != ListingStatus.Cancelled).Take(14))
        {
            int targetBookings = seededListing.CapacityUnit == ListingCapacityUnit.MaxWeight ? 2 : Math.Min(3, Math.Max(1, seededListing.Capacity));

            for (int b = 0; b < targetBookings; b++)
            {
                var traveler = travelers[(travelerIndex + b) % travelers.Count];
                if (traveler.UserId == seededListing.UUID)
                    continue;

                try
                {
                    bookingService.CreateBooking(traveler.UserId, seededListing.ListingId);
                    int? bookingId = bookingService.GetBookingId(traveler.UserId, seededListing.ListingId);
                    if (bookingId.HasValue)
                        seededBookingIds.Add((bookingId.Value, traveler.UserId, seededListing.ListingStatus));
                }
                catch
                {
                    // Ignore edge cases like capacity/weight limits while seeding sample data.
                }
            }

            travelerIndex++;
        }

        // --- Seed reviews for a subset of past bookings ---
        string[] reviewComments =
        [
            "Smooth experience and clear communication.",
            "Great value for the route and timing.",
            "Everything was on schedule and well organized.",
            "Host was responsive and helpful throughout.",
            "Would book again for this itinerary."
        ];

        int reviewIdx = 0;
        foreach (var (bookingId, userId, listingStatus) in seededBookingIds.Where(b => b.ListingStatus == ListingStatus.Past).Take(8))
        {
            if (reviewService.HasReview(bookingId))
                continue;

            int rating = rng.Next(3, 7);
            string comment = reviewComments[reviewIdx % reviewComments.Length];
            reviewService.CreateReview(userId, bookingId, rating, comment);
            reviewIdx++;
        }
    }
}

namespace space_booking_platform;

class Program
{
    static void Main(string[] args)
    {
        Database.Tables();
        Seeder.SeedIfEmpty();
        ViewHandler vh = new ViewHandler();
        vh.Run("Home");
    }
}
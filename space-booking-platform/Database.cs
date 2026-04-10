using System.Data.SQLite;

namespace space_booking_platform;

public abstract class Database
{
    private static string GetDbPath()
    {
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(projectRoot, "SpaceBookings.sqlite");
    }

    public static SQLiteConnection ConnectToDb()
    {
        string dbPath = GetDbPath();
        SQLiteConnection myConn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
        myConn.Open();
        return myConn;
    }

    public static void Tables()
    {
        SQLiteConnection myConn = ConnectToDb();
        
        string sql = "CREATE TABLE IF NOT EXISTS users(" +
                     "UUID INTEGER PRIMARY KEY," +
                     "username TEXT NOT NULL," +
                     "password TEXT NOT NULL," +
                     "weight INTEGER NOT NULL," +
                     "height INTEGER NOT NULL," +
                     "isOrganizer BOOL NOT NULL," +
                     "createdAt DATETIME NOT NULL)";

        SQLiteCommand command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();
        
        sql = "CREATE TABLE IF NOT EXISTS listings(" +
                     "listingID INTEGER PRIMARY KEY," +
                     "UUID INTEGER NOT NULL," +
                     "type TEXT NOT NULL," +
                     "title TEXT NOT NULL," +
                     "description TEXT NOT NULL," +
                     "transportMethod TEXT NOT NULL," +
                     "origin TEXT NOT NULL," +
                     "destination TEXT NOT NULL," +
                     "date DATETIME NOT NULL," +
                     "duration INTEGER NOT NULL," +
                     "durationType TEXT NOT NULL," +
                     "capacity INTEGER NOT NULL," +
                     "capacityUnit TEXT NOT NULL," +
                     "price INTEGER NOT NULL," +
                     "priceUnit TEXT NOT NULL," +
                     "createdAt DATETIME NOT NULL," +
                     "listingStatus TEXT NOT NULL," +
                     "FOREIGN KEY (UUID) REFERENCES users(UUID))";
        
        
        command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();
                
        sql = "CREATE TABLE IF NOT EXISTS bookings(" +
              "bookingID INTEGER PRIMARY KEY," +
              "UUID INTEGER NOT NULL," +
              "listingID INTEGER NOT NULL," +
              "bookingStatus TEXT NOT NULL," +
              "FOREIGN KEY (UUID) REFERENCES users(UUID)," +
              "FOREIGN KEY (listingID) REFERENCES listings(listingID))";
        
        command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();
        
        sql = "CREATE TABLE IF NOT EXISTS reviews(" +
              "reviewID INTEGER PRIMARY KEY," +
              "UUID INTEGER NOT NULL," +
              "bookingID INTEGER NOT NULL," +
              "rating INTEGER NOT NULL," +
              "comment TEXT," +
              "createdAt DATETIME NOT NULL," +
              "FOREIGN KEY (UUID) REFERENCES users(UUID))";
        
        command = new SQLiteCommand(sql, myConn);
        command.ExecuteNonQuery();

        myConn.Close();
    }
}
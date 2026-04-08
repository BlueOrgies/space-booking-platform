using System.Security.Cryptography;
using System.Text;
using System.Data.SQLite;
using space_booking_platform.Models;

namespace space_booking_platform;

public class UserService
{
    private static string HashPassword(string password)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public User Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        using SQLiteConnection conn = Database.ConnectToDb();

        using (SQLiteCommand checkCmd = new SQLiteCommand(
            "SELECT COUNT(*) FROM users WHERE username = @username", conn))
        {
            checkCmd.Parameters.AddWithValue("@username", username);
            long count = (long)checkCmd.ExecuteScalar()!;
            if (count > 0)
                throw new InvalidOperationException("Username is already taken.");
        }

        string hashedPassword = HashPassword(password);
        DateTime now = DateTime.UtcNow;

        using (SQLiteCommand insertCmd = new SQLiteCommand(
            "INSERT INTO users (username, password, weight, height, isOrganizer, createdAt) " +
            "VALUES (@username, @password, @weight, @height, @isOrganizer, @createdAt)", conn))
        {
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", hashedPassword);
            insertCmd.Parameters.AddWithValue("@weight", 0);
            insertCmd.Parameters.AddWithValue("@height", 0);
            insertCmd.Parameters.AddWithValue("@isOrganizer", 0);
            insertCmd.Parameters.AddWithValue("@createdAt", now.ToString("o"));
            insertCmd.ExecuteNonQuery();
        }

        using SQLiteCommand fetchCmd = new SQLiteCommand(
            "SELECT * FROM users WHERE UUID = last_insert_rowid()", conn);
        using SQLiteDataReader reader = fetchCmd.ExecuteReader();

        reader.Read();
        return MapUser(reader);
    }
    private static User MapUser(SQLiteDataReader reader) => new User
    {
        UserId = Convert.ToInt32(reader["UUID"]),
        Username = reader["username"].ToString()!,
        Password = reader["password"].ToString()!,
        Weight = Convert.ToInt32(reader["weight"]),
        Height = Convert.ToInt32(reader["height"]),
        IsOrganizer = Convert.ToBoolean(reader["isOrganizer"]),
        CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!)
    };
}

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


    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="username">The desired username.</param>
    /// <param name="password">The user's password (will be hashed).</param>
    /// <param name="weight">The user's weight.</param>
    /// <param name="height">The user's height.</param>
    /// <param name="isOrganizer">Whether the user is an organizer.</param>
    /// <returns>The registered User object.</returns>
    /// <exception cref="ArgumentException">Thrown if username or password is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the username is already taken.</exception>
    public User Register(string username, string password, int weight, int height, bool isOrganizer)
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
            insertCmd.Parameters.AddWithValue("@weight", weight);
            insertCmd.Parameters.AddWithValue("@height", height);
            insertCmd.Parameters.AddWithValue("@isOrganizer", isOrganizer ? 1 : 0);
            insertCmd.Parameters.AddWithValue("@createdAt", now.ToString("o"));
            insertCmd.ExecuteNonQuery();
        }

        using SQLiteCommand fetchCmd = new SQLiteCommand(
            "SELECT * FROM users WHERE UUID = last_insert_rowid()", conn);
        using SQLiteDataReader reader = fetchCmd.ExecuteReader();

        reader.Read();
        return MapUser(reader);
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>The User object if authentication is successful; otherwise, null.</returns>
    public User? Login(string username, string password)
    {
        string hashedPassword = HashPassword(password);

        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM users WHERE username = @username AND password = @password", conn);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", hashedPassword);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return MapUser(reader);
    }

    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The User object if found; otherwise, null.</returns>
    public User? GetById(int id)
    {
        using SQLiteConnection conn = Database.ConnectToDb();
        using SQLiteCommand cmd = new SQLiteCommand(
            "SELECT * FROM users WHERE UUID = @id", conn);

        cmd.Parameters.AddWithValue("@id", id);

        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

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

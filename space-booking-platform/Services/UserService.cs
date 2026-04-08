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
}

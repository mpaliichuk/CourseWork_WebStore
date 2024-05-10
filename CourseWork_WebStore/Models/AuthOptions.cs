using System;
using Microsoft.IdentityModel.Tokens;

namespace CourseWork_WebStore.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "OnlineShop";
        public const string AUDIENCE = "Users";
        public const int LIFETIME = 10;
        public const string KEY = "YourSecretKey12345678901234567890";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            if (KEY.Length < 32)
            {
                throw new ArgumentException("The JWT token key must have a size of at least 256 bits (32 bytes) for HS256 algorithm.");
            }

            return new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(KEY));
        }
    }
}

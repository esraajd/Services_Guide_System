using System.Security.Cryptography;
using System.Text;
using System;

namespace WebApplication1.Tool
{
    public class password
    {
        public static string Hashpassword(string password)
        {
            var sha = SHA256.Create();
            var asByteArray = Encoding.Default.GetBytes(password);
            var HashPAssword = sha.ComputeHash(asByteArray);
            return Convert.ToBase64String(HashPAssword); ;

        }
    }
}

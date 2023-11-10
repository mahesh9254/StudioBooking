using System.Security.Cryptography;
using System.Text;

namespace StudioBooking.Infrastructure
{
    public class Encryption
    {
        public static string Encrypt(string val)
        {
            var bytes = Encoding.UTF8.GetBytes(val);
            var encBytes = ProtectedData.Protect(bytes, Array.Empty<byte>(), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encBytes);
        }


        public static string Decrypt(string val)
        {
            var bytes = Convert.FromBase64String(val);
            var encBytes = ProtectedData.Unprotect(bytes, Array.Empty<byte>(), DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(encBytes);
        }

        public static string StringToBase64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string Base64ToString(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}

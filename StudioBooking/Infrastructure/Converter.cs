using System.Text.Json;

namespace StudioBooking.Infrastructure
{
    public class Converter
    {
        public static T ConvertJsonToObject<T>(string jsonString) where T : new()
        {
            return JsonSerializer.Deserialize<T>(jsonString) ?? new T();
        }
        public static T ConvertCookieToObject<T>(string cookie) where T : new()
        {
            return JsonSerializer.Deserialize<T>(Encryption.Base64ToString(cookie)) ?? new T();
        }
        public static string ConvertObjectToEncryptedJsonString(object obj)
        {
            return Encryption.StringToBase64(ConvertObjectToJsonString(obj));
        }

        public static string ConvertObjectToJsonString(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}

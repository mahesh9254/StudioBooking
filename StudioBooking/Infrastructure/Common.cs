using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using System.Globalization;
using System.Net;
using System.Text;
using static StudioBooking.Infrastructure.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace StudioBooking.Infrastructure
{
    public class Common
    {
        public const string SMSUrl = "https://smsstriker.com/API/internationalSMS.php";
        public const string SMSCountryAPI = "http://api.smscountry.com/SMSCwebservice_bulk.aspx";

        public string UploadedFile(IFormFile file, string path)
        {
            var uniqueFileName = string.Empty;
            if (file != null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(path, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public static string GetCookie(HttpRequest request, string key)
        {
            return request.Cookies[key];
        }

        public static void SetCookie(HttpResponse response, string key, string value, int? expireTime)
        {
            var option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = Defaults.GetDateTime().AddMinutes(expireTime.Value);
            else
                option.Expires = Defaults.GetDateTime().AddMinutes(15);
            response.Cookies.Append(key, value, option);
        }
        public static void Remove(HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }
    }
    public static class UserRole
    {
        public const string Basic = "Basic";
        public const string Moderator = "Moderator";
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
    }

    public static class GoogleAccount
    {
        public static string CalendarEmailAccount = "noreply@rbstudios.info";
    }

    public static class AppConfig
    {
        public const string ProfileImageUrl = "/lib/studiobooking/img/profile/";
        public const string CategoryImageUrl = "/lib/studiobooking/img/category/";
        public const string CategoryGearImageUrl = "/lib/studiobooking/img/category/gear/";
        public const string ServiceGalleryImageUrl = "/lib/studiobooking/img/gallery/";
        public const string WebsiteSettingsImageUrl = "/lib/studiobooking/img/website/";
        public const string ClientImageUrl = "/lib/studiobooking/img/client/";
        public const string TeamImageUrl = "/lib/studiobooking/img/team/";
        public const string GoolgeCalendarAPI = "/lib/CalendarAPI/rbstudio.json";
    }

    public static class MailIds
    {
        public const string RBStudioCC = "admin@rbstudios.info";
    }

    public class EventColor
    {
        public int Id { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }

        public static EventColor GetEventColor(string id)
        {
            return GetEventColors().FirstOrDefault(a => a.Id == int.Parse(id)) ?? new EventColor();
        }

        public static List<EventColor> GetEventColors()
        {
            return new List<EventColor>
            {
                new EventColor { Id = 1,Background= "#a4bdfc",Foreground="#1d1d1d"},
                new EventColor { Id = 2,Background= "#7ae7bf",Foreground="#1d1d1d"},
                new EventColor { Id = 3,Background= "#dbadff",Foreground="#1d1d1d"},
                new EventColor { Id = 4,Background= "#ff887c",Foreground="#1d1d1d"},
                new EventColor { Id = 5,Background= "#fbd75b",Foreground="#1d1d1d"},
                new EventColor { Id = 6,Background= "#ffb878",Foreground="#1d1d1d"},
                new EventColor { Id = 7,Background= "#46d6db",Foreground="#1d1d1d"},
                new EventColor { Id = 8,Background= "#e1e1e1",Foreground="#1d1d1d"},
                new EventColor { Id = 9,Background= "#5484ed",Foreground="#1d1d1d"},
                new EventColor { Id = 10,Background= "#51b749",Foreground="#1d1d1d"},
                new EventColor { Id = 11,Background= "#dc2127",Foreground="#1d1d1d"}
            };
        }
    }

    public static class Defaults
    {
        public const string ProfileImage = "/lib/assets/media/svg/avatars/029-boy-11.svg";
        public const string UploadImage = "/lib/Assets/resources/images/uploadIcon.png";
        public const string FileIcon = "/lib/Assets/resources/images/fileicon.png";
        public const string PasswordResetTemplate = @"Hi, Your password is updated successfully at %domain%.\nNew Password: %PWD%\nThank You!";
        public const string StudioAddress = "Venus Tower, 702/905, B Wing, Veera Desai Rd, Azad Nagar, Andheri West, Mumbai, Maharashtra 400053";
        public const string StartTime = "08:00 AM";
        public const string EndTime = "11:00 PM";
        public const string BookingPrefix = "RB/B/000000";
        public const string TransactionPrefix = "RB/B/000000";
        public const string CustomerPrefix = "RB/ID/000000";
        public const string DefaultCurrency = "INR";
        public const string DefaultDateFormat = "dd/MM/yyyy";

        public static object GetBookingStatusCSS(Enums.BookingStatus bookingStatus)
        {
            return bookingStatus == Enums.BookingStatus.Booked ? new { text = "Booked", css = "success" }
                                                : bookingStatus == Enums.BookingStatus.WaitingForApproval ? new { text = "Waiting For Approval", css = "dark" }
                                                : bookingStatus == Enums.BookingStatus.Pending ? new { text = "Pending", css = "warning" }
                                                : bookingStatus == Enums.BookingStatus.OnHold ? new { text = "On-Hold", css = "primary" }
                                                : bookingStatus == Enums.BookingStatus.ReScheduled ? new { text = "Re-Scheduled", css = "info" }
                                                : bookingStatus == Enums.BookingStatus.WaitingForCancellation ? new { text = "WaitingForCancellation", css = "dark" }
                                                : bookingStatus == Enums.BookingStatus.Cancelled ? new { text = "Cancelled", css = "danger" }
                                                : new { text = "Pending", css = "warning" };
        }

        public static object GetTransactionTypeCSS(Enums.TransactionType transactionType)
        {
            return transactionType == Enums.TransactionType.Debit ? new { text = "Debit", css = "danger" } : new { text = "Credit", css = "success" };
        }

        public static object GetTransactionStatusCSS(Enums.TransactionStatus transactionStatus)
        {
            return transactionStatus == Enums.TransactionStatus.Success ? new { text = "Success", css = "success" }
                                                : transactionStatus == Enums.TransactionStatus.Pending ? new { text = "Pending", css = "warning" }
                                                : transactionStatus == Enums.TransactionStatus.Failed ? new { text = "Failed", css = "danger" }
                                                : new { text = "Pending", css = "warning" };
        }

        public static object GetPaymentStatusCSS(Enums.PaymentStatus paymentStatus)
        {
            return paymentStatus == Enums.PaymentStatus.Pending ? new { text = "Pending", css = "danger" }
                                                : paymentStatus == Enums.PaymentStatus.Advance ? new { text = "Advance Paid", css = "warning" }
                                                : paymentStatus == Enums.PaymentStatus.FullPayment ? new { text = "Full Paid", css = "success" }
                                                : paymentStatus == Enums.PaymentStatus.RefundPending ? new { text = "Refund Pending", css = "info" }
                                                : paymentStatus == Enums.PaymentStatus.RefundCompleted ? new { text = "Refund Completed", css = "success" }
                                                : new { text = "Pending", css = "danger" };
        }

        public static DateTime GetLegecyDate()
        {
            return DateTime.UtcNow.AddYears(-5);
        }

        public static DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }

        public static DateTime ConvertDateTime(string datetime, string format)
        {
            return DateTime.Parse(datetime);
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        public static IList<T[]> GetChunks<T>(this IEnumerable<T> source, int batchsize)
        {
            IList<T[]> result = null;
            if (source != null && batchsize > 0)
            {
                var list = source as List<T> ?? source.ToList();
                if (list.Count > 0)
                {
                    result = new List<T[]>();
                    for (var index = 0; index < list.Count; index += batchsize)
                    {
                        var rangesize = Math.Min(batchsize, list.Count - index);
                        result.Add(list.GetRange(index, rangesize).ToArray());
                    }
                }
            }
            return result ?? Enumerable.Empty<T[]>().ToList();
        }

        public static string Sentencify(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string final = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                if (i != 0 && char.IsUpper(value[i]))
                {
                    if (!char.IsUpper(value[i - 1]))
                        final += " ";
                    else if (i < (value.Length - 1))
                    {
                        if (!char.IsUpper(value[i + 1]) && !((value.Length >= i && value[i + 1] == 's') ||
                                                             (value.Length >= i + 1 && value[i + 1] == 'e' && value[i + 2] == 's')))
                            final += " ";
                    }
                }
                final += value[i];
            }
            return final;
        }

        public static string? GetServiceTitle(string? serviceName)
        {
            return serviceName == "Rehearsal" ? "Jamming" : "Record";
        }
    }
}

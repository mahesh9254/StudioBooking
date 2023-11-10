using StudioBooking.Data.Models;
using StudioBooking.DTO;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Diagnostics.Contracts;

namespace StudioBooking.Infrastructure
{
    public interface IEmailNotification
    {
        Task<bool> SendEmail(WebsiteSettingDTO websiteSetting, EmailNotificationDTO emailNotification);
        Task<bool> SendUserRegisterNotification(string email, ApplicationUser user, WebsiteSettingDTO websiteSetting);
        Task<bool> SendBookingNotification(BookingDTO booking, ApplicationUser user, WebsiteSettingDTO websiteSetting);
        Task<bool> SendWalletUpdateNotification(WalletTransaction wallet, ApplicationUser user, WebsiteSettingDTO websiteSetting);
        Task<bool> SendReScheduleCancellationNotification(ScheduleRequest scheduleRequest, BookingDTO booking, ApplicationUser user, WebsiteSettingDTO websiteSetting);
        Task<bool> SendPaymentLinkNotification(Booking booking, ApplicationUser user, WebsiteSettingDTO websiteSetting);
    }

    public class EmailNotification : IEmailNotification
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static string? _userRegistration;
        private static string? _addFundsInWalletTemplate;

        public EmailNotification(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _userRegistration = _webHostEnvironment.WebRootPath + ("/templates/userRegistration.html").Replace("/", "\\");
            _addFundsInWalletTemplate = _webHostEnvironment.WebRootPath + AppConfig.ServiceGalleryImageUrl.Replace("/", "\\");
        }

        public async Task<bool> SendEmail(WebsiteSettingDTO websiteSetting, EmailNotificationDTO emailNotification)
        {
            bool result;
            try
            {
                var myMessage = new MailMessage(websiteSetting.EmailAccount, emailNotification.EmailId)
                {
                    Subject = emailNotification.Subject,
                    Body = emailNotification.EmailBody,
                    Priority = MailPriority.High,
                    IsBodyHtml = true
                };

                if (emailNotification.Docs.Count > 0)
                    await AddAttachmentsAsync(myMessage, emailNotification.Docs);

                if (!string.IsNullOrEmpty(emailNotification.EmailCc))
                    AddEmailCcRecipients(myMessage, emailNotification);

                //if (!string.IsNullOrEmpty(emailNotification.EmailBcc))
                //    AddEmailBccRecipients(myMessage, emailNotification);

                var basicAuthentication = new NetworkCredential(websiteSetting.EmailAccountUserId, websiteSetting.EmailAccountPassword);

                var emailClient = new SmtpClient(websiteSetting.EmailAccountSmtp)
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = basicAuthentication
                };
                await emailClient.SendMailAsync(myMessage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> SendUserRegisterNotification(string email, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {
            bool result;
            try
            {
                string body;
                using (var reader = new StreamReader(_userRegistration))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("%Name%", user.FirstName + " " + user.LastName);
                body = body.Replace("%Email%", email);
                var emailNotification = new EmailNotificationDTO
                {
                    EmailId = email,
                    Subject = "Your account on " + websiteSetting.Name,
                    EmailBody = body,
                    EmailBcc = MailIds.RBStudioCC
                };
                result = await SendEmail(websiteSetting, emailNotification);
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> SendEmailAmountAddedInWallet(EmailNotificationDTO emailNotification, WebsiteSettingDTO websiteSetting, Transaction trasaction, decimal walletamount)
        {
            string body;
            using (var reader = new StreamReader(_addFundsInWalletTemplate + ("~/Content/Templates/AddFundsInWalletTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("%Name%", emailNotification.Name);
            body = body.Replace("%TxnID%", trasaction.Id.ToString());
            body = body.Replace("%TransactionDate%", trasaction.CreatedDate.ToString("yyyy MM dd"));
            //body = body.Replace("%TransactionFee%", trasaction.TransactionFee.ToString());
            body = body.Replace("%Amount%", walletamount.ToString());
            body = body.Replace("%TotlaAmount%", trasaction.Amount.ToString());
            emailNotification.EmailBody = body;
            return await SendEmail(websiteSetting, emailNotification);
        }

        private static void AddEmailBccRecipients(MailMessage myMessage, EmailNotificationDTO emailNotification)
        {
            foreach (var bcc in emailNotification.EmailBcc.Split(';'))
                myMessage.Bcc.Add(new MailAddress(bcc));
        }

        private static void AddEmailCcRecipients(MailMessage myMessage, EmailNotificationDTO emailNotification)
        {
            foreach (var cc in emailNotification.EmailCc.Split(';'))
                myMessage.CC.Add(new MailAddress(cc));

        }

        private static async Task AddAttachmentsAsync(MailMessage myMessage, List<string> attachmentList)
        {
            foreach (var attachment in attachmentList)
            {
                if (string.IsNullOrEmpty(attachment)) continue;

                var filename = attachment.Split('/').LastOrDefault();

                if (attachment.Contains("\\"))
                    filename = attachment.Split('\\').LastOrDefault();

                //var response = new WebClient().OpenRead(attachment);
                using HttpResponseMessage response = await new HttpClient().GetAsync(attachment);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                myMessage.Attachments.Add(new Attachment(responseBody ?? Stream.Null, filename));
            }
        }

        public async Task<bool> SendBookingNotification(BookingDTO booking, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {
            bool result;
            try
            {
                var template = _webHostEnvironment.WebRootPath + ("/templates/booking.html").Replace("/", "\\");
                string body;
                using (var reader = new StreamReader(template))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("%Name%", user.FirstName + " " + user.LastName);
                body = body.Replace("%StudioName%", booking.CategoryName);
                body = body.Replace("%BookingID%", booking.Id.ToString(Defaults.BookingPrefix));
                body = body.Replace("%OrderDate%", booking.BookingDate);
                body = body.Replace("%ServiceName%", booking.ServiceName);
                body = body.Replace("%Timing%", TimeOnly.FromDateTime(DateTime.Parse(booking.StartTime)) + " - " + TimeOnly.FromDateTime(DateTime.Parse(booking.EndTime)));
                body = body.Replace("%Rate%", Math.Round(booking.RatePerHour, 2).ToString());
                body = body.Replace("%Total%", booking.Total.ToString());
                body = body.Replace("%Advance%", booking.AdvancePaid.ToString());
                body = body.Replace("%ServiceTitle%", booking.ServiceTitle);
                body = body.Replace("%StudioName%", booking.CategoryName);

                var emailNotification = new EmailNotificationDTO
                {
                    EmailId = user.Email,
                    Subject = "Your booking of " + booking.ServiceName + " - " + booking.CategoryName + " on " + websiteSetting.Name + " has been placed successfully.",
                    EmailBody = body,
                    EmailBcc = MailIds.RBStudioCC
                };
                result = await SendEmail(websiteSetting, emailNotification);
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public Task<bool> SendWalletUpdateNotification(WalletTransaction wallet, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPaymentReminderNotification(Booking booking, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendReScheduleCancellationNotification(ScheduleRequest scheduleRequest, BookingDTO booking, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {

            bool result;
            try
            {
                var template = _webHostEnvironment.WebRootPath + (scheduleRequest.RequestType == (int)Enums.RequestType.Cancel ? "/templates/cancel.html" : "/templates/reschedule.html").Replace("/", "\\");
                string body;
                using (var reader = new StreamReader(template))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("%Name%", user.FirstName + " " + user.LastName);
                if (scheduleRequest.RequestType == (int)Enums.RequestType.ReSchedule)
                {
                    body = body.Replace("%TIME%", TimeOnly.FromDateTime(DateTime.Parse(scheduleRequest.StartTime)) + " - " + TimeOnly.FromDateTime(DateTime.Parse(scheduleRequest.EndTime)));
                    body = body.Replace("%OldDate%", scheduleRequest.BookingDate?.ToString(Defaults.DefaultDateFormat));
                    body = body.Replace("%NewDate%", scheduleRequest.RequestDate?.ToString(Defaults.DefaultDateFormat));
                    body = body.Replace("%NEWTIME%", TimeOnly.FromDateTime(scheduleRequest.RequestStartTime ?? DateTime.MinValue) + " - " + TimeOnly.FromDateTime(scheduleRequest.RequestEndTime ?? DateTime.MinValue));
                }
                else
                {
                    body = body.Replace("%TIME%", TimeOnly.FromDateTime(DateTime.Parse(booking.StartTime)) + " - " + TimeOnly.FromDateTime(DateTime.Parse(booking.EndTime)));
                    body = body.Replace("%BookingDate%", booking.BookingDate);
                    body = body.Replace("%CancellationAmount%", (Math.Round((booking.Total - booking.AdvancePaid) == 0 ? booking.Total / 2 : 0), 2).ToString());
                }
                var emailNotification = new EmailNotificationDTO
                {
                    EmailId = user.Email,
                    Subject = "Your booking of " + booking.ServiceName + " - " + booking.CategoryName + " on " + websiteSetting.Name + " has been " + (scheduleRequest.RequestType == (int)Enums.RequestType.ReSchedule ? "rescheduled" : "cancelled") + " successfully.",
                    EmailBody = body,
                    EmailBcc = MailIds.RBStudioCC
                };
                result = await SendEmail(websiteSetting, emailNotification);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public Task<bool> SendPaymentLinkNotification(Booking booking, ApplicationUser user, WebsiteSettingDTO websiteSetting)
        {
            throw new NotImplementedException();
        }
    }
}

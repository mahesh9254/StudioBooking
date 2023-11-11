using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.CodeAnalysis;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using System;

namespace StudioBooking.Infrastructure
{
    public interface IGoogleCalendar
    {
        Task<string> AddCalenderEventAsync(EventDTO eventDetail);
        Task<Event> UpdateCalenderEvents(string eventId, EventDTO eventDetail);
        Task DeletCalendarEvents(string emailAddress, string eventId);
    }
    public class GoogleCalendar : IGoogleCalendar
    {
        private readonly string ApplicationName = "Studio Web";
        private readonly string ClientId = "883338052842-8c4u0bk1mr8dgiudatgu0c59sg1di69g.apps.googleusercontent.com";
        private readonly string ClientSecret = "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";
        private readonly string APIKey = "AIzaSyBvMSIOf3HJg5KnUyT6p2pU1JmhiEFMx0c";
        private readonly ClientSecrets _clientSecrets;
        private string _refreshToken = string.Empty;
        private readonly string _filePath;
        private static readonly string[] Scopes =
        {
            CalendarService.Scope.Calendar,
            CalendarService.Scope.CalendarReadonly
        };
        private readonly ILogger<GoogleCalendar> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GoogleCalendar(ILogger<GoogleCalendar> logger, IWebHostEnvironment webHostEnvironment)
        {
            _clientSecrets = new ClientSecrets()
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret
            };
            _logger = logger;
            _filePath = webHostEnvironment.WebRootPath + AppConfig.GoolgeCalendarAPI.Replace("/", "\\");
        }


        private async Task<UserCredential> GetUserCredential()
        {
            try
            {
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(_clientSecrets, Scopes, GoogleAccount.CalendarEmailAccount, CancellationToken.None, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to UserCredential Initialization: " + ex.ToString(), ex);
            }
            return null;
        }


        private async Task<UserCredential> GetGoogleUserCredentialByRefreshToken()
        {
            TokenResponse respnseToken = null;
            UserCredential credential = null;
            try
            {
                // Get a new IAuthorizationCodeFlow instance
                IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = _clientSecrets,
                    Scopes = Scopes
                });

                respnseToken = new TokenResponse() { RefreshToken = _refreshToken };

                // Get a new Credential instance                
                if (flow != null && respnseToken != null)
                {
                    credential = new UserCredential(flow, "user", respnseToken);
                }

                // Get a new Token instance
                if (credential != null)
                {
                    bool success = await credential.RefreshTokenAsync(CancellationToken.None);
                }

                // Set the new Token instance
                if (credential.Token != null)
                {
                    string newRefreshToken = credential.Token.RefreshToken;
                }
            }
            catch (Exception ex)
            {
                credential = null;
                _logger.LogError("UserCredential failed: " + ex.ToString(), ex);
            }
            return credential;
        }

        public CalendarService GetCalendarService()
        {
            CalendarService calendarService;
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes).CreateWithUser(GoogleAccount.CalendarEmailAccount);
                }
                calendarService = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
            }
            catch (Exception ex)
            {
                calendarService = null;
                _logger.LogError("Calendar service failed: " + ex.ToString());
            }
            return calendarService;
        }

        public async Task<string> AddCalenderEventAsync(EventDTO eventDetail)
        {
            string eventId = string.Empty;
            try
            {
                var calendarService = GetCalendarService();
                if (calendarService != null)
                {
                    var list = await calendarService.CalendarList.List().ExecuteAsync();
                    var calendar = list.Items.SingleOrDefault(c => c.Summary == eventDetail.CalendarName.Trim());
                    if (calendar != null)
                    {
                        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        var startTime = DateTime.SpecifyKind(eventDetail.Date.ToDateTime(eventDetail.StartTime), DateTimeKind.Unspecified);
                        var startTimeUTC = TimeZoneInfo.ConvertTime(startTime, timeZone, TimeZoneInfo.Utc);
                        var endTime = DateTime.SpecifyKind(eventDetail.EndDate.ToDateTime(eventDetail.EndTime), DateTimeKind.Unspecified);
                        var endTimeUTC = TimeZoneInfo.ConvertTime(endTime, timeZone, TimeZoneInfo.Utc);                      
                        var calenderEvent = new Event
                        {
                            Summary = eventDetail.Title,
                            Description = eventDetail.Description,
                            Location = Defaults.StudioAddress, //eventDetail.Studio,
                            Start = new EventDateTime
                            {
                                DateTime = startTimeUTC,
                                //TimeZone = calendar.TimeZone
                            },
                            End = new EventDateTime
                            {
                                DateTime = endTimeUTC,
                                //TimeZone = calendar.TimeZone,
                            },
                            Recurrence = new List<string>(),
                            //Set Remainder
                            Reminders = new Event.RemindersData()
                            {
                                UseDefault = false,
                                Overrides = new EventReminder[]
                                {
                                    new EventReminder() { Method = "email", Minutes = 24 * 60 },
                                    new EventReminder() { Method = "popup", Minutes = 24 * 60 }
                                }
                            },                            
                        };
                        //if (!string.IsNullOrEmpty(eventDetail.ColorId))
                        //    calenderEvent.ColorId = eventDetail.ColorId;

                        #region Attendees
                        //Set Attendees
                        var attendees = new List<EventAttendee>();
                        //{
                        //    new EventAttendee() { Email = GoogleAccount.CalendarEmailAccount,ResponseStatus= "accepted", DisplayName="R & B Studios" }
                        //};
                        //if (!string.IsNullOrEmpty(eventDetail.Email))
                        //{
                        //    attendees.Add(new EventAttendee() { Email = eventDetail.Email });
                        //}
                        calenderEvent.Attendees = attendees;
                        #endregion
                        var newEventRequest = calendarService.Events.Insert(calenderEvent,calendar.Id);
                        newEventRequest.SendNotifications = true;
                        var eventResult = await newEventRequest.ExecuteAsync();
                        eventId = eventResult.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                eventId = string.Empty;
                _logger.LogError(ex.Message);
            }
            return eventId;
        }
        public async Task<Event> UpdateCalenderEvents(string eventId, EventDTO eventDetail)
        {
            Event eventResult = null;
            try
            {
                var calendarService = GetCalendarService();
                if (calendarService != null)
                {
                    var list = await calendarService.CalendarList.List().ExecuteAsync();
                    var calendar = list.Items.SingleOrDefault(c => c.Summary == eventDetail.CalendarName.Trim());
                    if (calendar != null)
                    {
                        // Define parameters of request
                        EventsResource.ListRequest request = calendarService.Events.List(calendar.Id);
                        request.TimeMin = DateTime.Now;
                        request.ShowDeleted = false;
                        request.SingleEvents = true;
                        request.MaxResults = 500;
                        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                        // Get selected event
                        Events events = await request.ExecuteAsync();
                        var selectedEvent = events.Items.FirstOrDefault(c => c.Id == eventId);
                        if (selectedEvent != null)
                        {
                            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                            var startTime = DateTime.SpecifyKind(eventDetail.Date.ToDateTime(eventDetail.StartTime), DateTimeKind.Unspecified);
                            var startTimeUTC = TimeZoneInfo.ConvertTime(startTime, timeZone, TimeZoneInfo.Utc);
                            var endTime = DateTime.SpecifyKind(eventDetail.EndDate.ToDateTime(eventDetail.EndTime), DateTimeKind.Unspecified);
                            var endTimeUTC = TimeZoneInfo.ConvertTime(endTime, timeZone, TimeZoneInfo.Utc);
                            selectedEvent.Summary = eventDetail.Title;
                            selectedEvent.Description = eventDetail.Description;
                            selectedEvent.Location = Defaults.StudioAddress; //eventDetail.Studio;
                            selectedEvent.Start = new EventDateTime
                            {
                                DateTime = startTimeUTC
                            };
                            selectedEvent.End = new EventDateTime
                            {
                                DateTime = endTimeUTC
                            };                            
                        }

                        var updateEventRequest = calendarService.Events.Update(selectedEvent, calendar.Id, eventId);
                        updateEventRequest.SendNotifications = true;
                        eventResult = await updateEventRequest.ExecuteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                eventResult = null;
                _logger.LogError(ex.ToString());
            }
            return eventResult;
        }
        public async Task DeletCalendarEvents(string calenderName, string eventId)
        {
            string result = string.Empty;
            try
            {
                var calendarService = GetCalendarService();
                if (calendarService != null)
                {
                    var list = await calendarService.CalendarList.List().ExecuteAsync();
                    var calendar = list.Items.FirstOrDefault(c => c.Summary == calenderName.Trim());
                    if (calendar != null)
                    {
                        // Define parameters of request
                        EventsResource.ListRequest request = calendarService.Events.List(calendar.Id);
                        request.TimeMin = DateTime.Now;
                        request.ShowDeleted = false;
                        request.SingleEvents = true;
                        request.MaxResults = 500;
                        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                        // Get selected event
                        Events events = await request.ExecuteAsync();
                        var selectedEvent = events.Items.FirstOrDefault(c => c.Id == eventId);
                        if (selectedEvent != null)
                        {
                            var deleteEventRequest = calendarService.Events.Delete(calendar.Id, eventId);
                            deleteEventRequest.SendNotifications = true;
                            await deleteEventRequest.ExecuteAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
                _logger.LogError(ex.ToString());
            }
        }
    }
}

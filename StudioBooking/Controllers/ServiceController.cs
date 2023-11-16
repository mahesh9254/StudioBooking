using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using StudioBooking.ViewModels;
using ServiceViewModel = StudioBooking.ViewModels.ServiceViewModel;

namespace StudioBooking.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _context = applicationDbContext;
        }
        public async Task<IActionResult> Index(int? serviceId)
        {
            var servicePriceList = new List<ServicePriceDTO>();
            var servicePrices = serviceId != null ? await ServicePriceDTO.GetServicePricesByService(_context, serviceId ?? 0) : await ServicePriceDTO.GetServicePrices(_context);
            foreach (var service in servicePrices.Select(s => s.ServiceId).Distinct())
            {
                var minServicePrice = servicePrices.Where(s => s.ServiceId == service).MinBy(s => s.Price) ?? new ServicePriceDTO();
                servicePriceList.Add(minServicePrice);
            }
            var studioViewModel = new StudioViewModel
            {
                ServicePrices = servicePriceList,
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var servicePrices = await ServicePriceDTO.GetServicePricesByService(_context, id);
            var serviceViewModel = new ServiceViewModel
            {
                Cart = new CartDTO { ServicePriceId = id },
                ServicePrice = servicePrices.MinBy(s => s.Price)
            };
            return View(serviceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookedTimeSlots(int id, string date, string userId, int type = 1)
        {
            userId = string.IsNullOrEmpty(userId) ? GetUserId() : userId;
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
            var categoryBookings = await BookingDTO.GetBookingsByCategoryId(_context, id, Defaults.ConvertDateTime(date, Defaults.DefaultDateFormat));
            
            var timeslotes = new List<string>();
            foreach (var bookings in categoryBookings.Where(b=> b.BookingStatus != Enums.BookingStatus.OnHold && b.BookingStatus != Enums.BookingStatus.Cancelled && b.BookingStatus!= Enums.BookingStatus.Failed && (type == 1 || b.CustomerId != customer.Id)))
            {
                for (DateTime appointment = DateTime.Parse(bookings.StartTime); appointment < DateTime.Parse(bookings.EndTime); appointment = appointment.AddHours(1))
                {                                    
                   timeslotes.Add(appointment.ToString("HH:mm"));
                }
            }            
            return Ok(timeslotes);
        }
        [HttpGet]
        public async Task<IActionResult> GetBookedEndTimeSlots(int id, string date, string userId, int type = 1)
        {
            userId = string.IsNullOrEmpty(userId) ? GetUserId() : userId;
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
            var categoryBookings = await BookingDTO.GetEndBookingsByCategoryId(_context, id, Defaults.ConvertDateTime(date, Defaults.DefaultDateFormat));

            var timeslotes = new List<string>();
            foreach (var bookings in categoryBookings.Where(b => b.BookingStatus != Enums.BookingStatus.OnHold && b.BookingStatus != Enums.BookingStatus.Cancelled && b.BookingStatus != Enums.BookingStatus.Failed && (type == 1 || b.CustomerId != customer.Id)))
            {                
                DateTime appointment = DateTime.Parse(bookings.EndTime);
                timeslotes.Add(appointment.ToString("HH:mm"));                
            }
            return Ok(timeslotes);
        }
        [HttpGet]
        public async Task<IActionResult> GetBookedStartEndTimeSlots(int id, string date, string userId, int type = 1)
        {
            userId = string.IsNullOrEmpty(userId) ? GetUserId() : userId;
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);            
            var categoryBookings = await BookingDTO.GetEndBookingsByCategoryId(_context, id, DateTime.Parse(date));
            var timeslotes = new List<object>(); 
            
            foreach (var bookings in categoryBookings.Where(b => b.BookingStatus != Enums.BookingStatus.OnHold && b.BookingStatus != Enums.BookingStatus.Cancelled && b.BookingStatus != Enums.BookingStatus.Failed && (type == 1 || b.CustomerId != customer.Id)))
            {                
                timeslotes.Add(new { start = DateTime.Parse(bookings.StartTime).ToString("HH:mm"), end = DateTime.Parse(bookings.EndTime).ToString("HH:mm"), startDate = bookings.BookingDate, endDate = bookings.BookingEndDate });
            }
            return Ok(timeslotes);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(ServiceViewModel model)
        {
            var activeUserCarts = await _context.Carts.Include(c => c.Customer).FirstOrDefaultAsync(c => c.IsActive && !(c.IsCheckedOut ?? false) && c.Customer.UserId == GetUserId());
            if (activeUserCarts != null)
            {
                activeUserCarts.IsActive = false;
                activeUserCarts.IsDelete = true;
                activeUserCarts.ModifiedBy = GetUserId();
                activeUserCarts.ModifiedDate = Defaults.GetDateTime();
            }

            var customer = await CustomerDTO.GetCustomer(_context, GetUserId());
            var cart = CartDTO.GetCart(model.Cart ?? new CartDTO(), customer.Id, GetUserId());
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            model.Cart.Id = cart.Id;
            Common.SetCookie(Response, "_crt", Encryption.Encrypt(Converter.ConvertObjectToJsonString(model.Cart)), 120);
            return RedirectToAction("Checkout", "Booking");
        }
    }
}

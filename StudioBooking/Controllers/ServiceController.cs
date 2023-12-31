﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using StudioBooking.ViewModels;

using System.Globalization;

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
			var splitDate = date.Split('-').Select(Int32.Parse).ToList();
			DateTime dtFormatted = new(splitDate[2], splitDate[1], splitDate[0]);
			var categoryBookings = await BookingDTO.GetBookingsByCategoryId(_context, id, dtFormatted);

			var timeslotes = new List<string>();
			foreach (var bookings in categoryBookings.Where(b => b.BookingStatus != Enums.BookingStatus.OnHold && b.BookingStatus != Enums.BookingStatus.Cancelled && b.BookingStatus != Enums.BookingStatus.Failed && (type == 1 || b.CustomerId != customer.Id)))
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
			var splitDate = date.Split('-').Select(Int32.Parse).ToList();
			DateTime dtFormatted = new(splitDate[2], splitDate[1], splitDate[0]);
			var categoryBookings = await BookingDTO.GetEndBookingsByCategoryId(_context, id, dtFormatted);

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
			var splitDate = date.Split('-').Select(Int32.Parse).ToList();
			DateTime dtFormatted = new(splitDate[2], splitDate[1], splitDate[0]);
			var categoryBookings = await BookingDTO.GetEndBookingsByCategoryId(_context, id, dtFormatted);
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
			//var oldTotalHours = (booking.BookingEndDate.Add(TimeSpan.Parse(booking.EndTime)) - booking.BookingDate.Add(TimeSpan.Parse(booking.StartTime))).TotalHours;
			var existingBooking = _context.Bookings;
			foreach (var m in existingBooking)
			{
				var splitStartDate = model.Cart.BookingDate.Split('-').Select(Int32.Parse).ToList();
				DateTime dtStartFormatted = new(splitStartDate[2], splitStartDate[1], splitStartDate[0]);
				var splitEndDate = model.Cart.BookingEndDate.Split('-').Select(Int32.Parse).ToList();
				DateTime dtEndFormatted = new(splitEndDate[2], splitEndDate[1], splitEndDate[0]);

				if (m.BookingDate.Add(TimeSpan.Parse(m.StartTime)) >= (dtStartFormatted.Add(TimeSpan.Parse(model.Cart.StartTime)))
					&& m.BookingEndDate.Add(TimeSpan.Parse(m.EndTime)) <= dtEndFormatted.Add(TimeSpan.Parse(model.Cart.EndTime)))
				{
					return RedirectToAction("Overlapping", "Booking");
				}

				//var startDate = m.BookingDate.Add(TimeSpan.Parse(m.StartTime));
				//var endDate = DateTime.ParseExact(m.BookingEndDate + " " + m.EndTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture);
				//var cartStartDate = DateTime.ParseExact(model.Cart.BookingDate + " " + model.Cart.StartTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture);
				//var cartEndDate = DateTime.ParseExact(model.Cart.BookingEndDate + " " + model.Cart.EndTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture);

				//if (startDate >= cartStartDate && endDate <= cartEndDate)
				//{
				//	return RedirectToAction("Overlapping", "Booking");
				//}
			}
			//if (existingBooking != null) {  }
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

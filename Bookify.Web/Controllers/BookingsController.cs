using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // -----------------------------
        // GET: Create Booking
        // -----------------------------
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            Booking model = new Booking
            {
                RoomId = room.Id,
                UserId = user.Id,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            ViewBag.Room = room;
            return View(model);
        }

        // -----------------------------
        // POST: Create Booking
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking model)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
            if (room == null) return NotFound();

            int nights = (model.CheckOutDate - model.CheckInDate).Days;

            if (nights <= 0)
            {
                ModelState.AddModelError("", "Checkout date must be after Check-in date.");
                ViewBag.Room = room;
                return View(model);
            }

            model.TotalPrice = nights * room.RoomType.Price;
            model.UserId = _userManager.GetUserId(User);
            model.PaymentStatus = "Pending";

            await _unitOfWork.Bookings.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("CompleteBooking", new { id = model.Id });
        }

        // -----------------------------
        // GET: Complete Booking
        // -----------------------------
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);
            booking.Room.RoomType = booking.Room.RoomType;

            return View(booking);
        }

        // -----------------------------
        // POST: Complete Booking
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBooking(int id, string paymentMethod)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.PaymentStatus = paymentMethod == "Online" ? "Paid" : "Pending";

            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                StripeTransactionId = paymentMethod == "Online" ? Guid.NewGuid().ToString() : null,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Payments.AddAsync(payment);

            var room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);
            room.Status = "Booked";

            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MyBookings", "Booking");

        }
    }
}

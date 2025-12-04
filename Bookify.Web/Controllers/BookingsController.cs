using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Start(int roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
                return NotFound();

            return RedirectToAction("Create", new { roomId = roomId });
        }

        // GET: Booking/Create
        [Authorize]   // ⛔ يمنع أي حد مش مسجل
        
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            Booking model = new Booking
            {
                RoomId = room.Id,
                UserId = user.Id,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            ViewBag.Room = room;
            ViewBag.User = user;

            return View(model);
        }





        // POST: Booking/Create
        [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking model)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
        if (room == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Room = room;
            return View(model);
        }

        // 🟢 حساب عدد الليالي
        int nights = (model.CheckOutDate - model.CheckInDate).Days;

        if (nights <= 0)
        {
            ModelState.AddModelError("", "Checkout date must be after check-in date.");
            ViewBag.Room = room;
            return View(model);
        }

        // 🟢 حساب السعر
        model.TotalPrice = nights * room.RoomType.Price;

        // 🟢 ربط الحجز بالمستخدم الحالي
        model.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

        // 🔴 حالة الدفع الافتراضية
        model.PaymentStatus = "Pending";

        await _unitOfWork.Bookings.AddAsync(model);
        await _unitOfWork.CompleteAsync();

        // تحويل لصفحة الدفع
        return RedirectToAction("Checkout", "Payments", new { bookingId = model.Id });
    }
        [Authorize]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return NotFound();

            var room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);

            ViewBag.RoomPrice = room.RoomType.Price;

            return View(booking);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBooking(int id, string paymentMethod)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return NotFound();

            // 🔵 1) تحديث حالة الدفع
            booking.PaymentStatus = paymentMethod == "Online" ? "Paid" : "Pending";

            // 🔵 2) إنشاء عملية الدفع
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                StripeTransactionId = paymentMethod == "Online" ? Guid.NewGuid().ToString() : null,
                CreatedAt = DateTime.Now   // ⭐ لازم عشان الموديل عندك بيطلبه
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // 🔵 3) تحديث الغرفة → Booked
            var room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);
            room.Status = "Booked";

            // 🔵 4) حفظ التغييرات
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MyBookings", "Home");
        }




        // My Reservations
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _unitOfWork.Bookings.FindAsync(x => x.UserId == userId);

            return View(bookings);
        }
    }
}

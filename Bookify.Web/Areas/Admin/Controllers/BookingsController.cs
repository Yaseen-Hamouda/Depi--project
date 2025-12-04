using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Admin/Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            return View(bookings);
        }

        // GET: Admin/Bookings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "FullName");

            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            ViewBag.Rooms = new SelectList(rooms, "Id", "RoomNumber");

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking model)
        {
            // ⚠️ validation
            if (model.CheckOutDate <= model.CheckInDate)
                ModelState.AddModelError("", "Check-out date must be after check-in date.");

            // ❌ check room availability
            var overlap = await _unitOfWork.Bookings.FindAsync(b =>
                b.RoomId == model.RoomId &&
                model.CheckInDate < b.CheckOutDate &&
                model.CheckOutDate > b.CheckInDate
            );

            if (overlap.Any())
                ModelState.AddModelError("", "This room is already booked for the selected dates.");

            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "FullName");
                ViewBag.Rooms = new SelectList(await _unitOfWork.Rooms.GetAllAsync(), "Id", "RoomNumber");
                return View(model);
            }

            // حساب السعر تلقائي
            var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
            int nights = (model.CheckOutDate - model.CheckInDate).Days;
            model.TotalPrice = nights * room.RoomType.Price;

            model.PaymentStatus = "Pending";

            await _unitOfWork.Bookings.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "FullName", booking.UserId);
            ViewBag.Rooms = new SelectList(await _unitOfWork.Rooms.GetAllAsync(), "Id", "RoomNumber", booking.RoomId);

            return View(booking);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking model)
        {
            if (model.CheckOutDate <= model.CheckInDate)
                ModelState.AddModelError("", "Check-out date must be after check-in date.");

            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(_userManager.Users.ToList(), "Id", "FullName", model.UserId);
                ViewBag.Rooms = new SelectList(await _unitOfWork.Rooms.GetAllAsync(), "Id", "RoomNumber", model.RoomId);
                return View(model);
            }

            // حساب السعر بعد التعديل
            var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
            int nights = (model.CheckOutDate - model.CheckInDate).Days;
            model.TotalPrice = nights * room.RoomType.Price;

            _unitOfWork.Bookings.Update(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Delete Confirm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking != null)
            {
                _unitOfWork.Bookings.Remove(booking);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

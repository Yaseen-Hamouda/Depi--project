using Bookify.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            // Summary cards
            var allRooms = (await _unitOfWork.Rooms.GetAllAsync()).ToList();
            var allBookings = (await _unitOfWork.Bookings.GetAllAsync()).ToList();
            var allPayments = (await _unitOfWork.Payments.GetAllAsync()).ToList();

            var totalRooms = allRooms.Count;
            var availableRooms = allRooms.Count(r => string.Equals(r.Status, "Available", StringComparison.OrdinalIgnoreCase));
            var totalBookings = allBookings.Count;
            var pendingPayments = allPayments.Count(p => p.CreatedAt == default || !string.Equals(
                (allBookings.FirstOrDefault(b => b.Id == p.BookingId)?.PaymentStatus ?? ""), "Paid", StringComparison.OrdinalIgnoreCase));

            // Recent bookings (most recent 10)
            var recentBookings = allBookings
                .OrderByDescending(b => b.CheckInDate)
                .Take(10)
                .ToList();

            var vm = new AdminDashboardViewModel
            {
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                TotalBookings = totalBookings,
                PendingPayments = pendingPayments,
                RecentBookings = recentBookings
            };

            return View(vm);
        }

        // JSON endpoint for RoomTypes distribution (pie)
        [HttpGet]
        public async Task<IActionResult> RoomTypesDistribution()
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            var distribution = rooms
                .GroupBy(r => r.RoomType?.Name ?? "Unknown")
                .Select(g => new { name = g.Key, count = g.Count() })
                .ToList();

            return Json(distribution);
        }

        // JSON endpoint for bookings per month (last 6 months)
        [HttpGet]
        public async Task<IActionResult> BookingsPerMonth(int months = 6)
        {
            var bookings = (await _unitOfWork.Bookings.GetAllAsync()).ToList();

            var results = new List<object>();
            var now = DateTime.UtcNow;
            for (int i = months - 1; i >= 0; i--)
            {
                var dt = now.AddMonths(-i);
                var year = dt.Year;
                var month = dt.Month;

                var count = bookings.Count(b => b.CheckInDate.Year == year && b.CheckInDate.Month == month);

                var label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month) + " " + year;
                results.Add(new { label, count });
            }

            return Json(results);
        }
    }

    // Simple ViewModel for the dashboard
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalBookings { get; set; }
        public int PendingPayments { get; set; }

        public IEnumerable<Bookify.Core.Models.Booking> RecentBookings { get; set; } = Enumerable.Empty<Bookify.Core.Models.Booking>();
    }
}

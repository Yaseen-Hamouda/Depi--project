using Bookify.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bookify.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // ------- 1. Summary Cards -------
            ViewBag.RoomsCount = (await _unitOfWork.Rooms.GetAllAsync()).Count();
            ViewBag.BookingsToday = (await _unitOfWork.Bookings.FindAsync(b => b.CheckInDate.Date == DateTime.Today)).Count();
            ViewBag.TotalRevenue = (await _unitOfWork.Payments.GetAllAsync()).Sum(p => p.Amount);
            ViewBag.UsersCount = _userManager.Users.Count();

            // ------- 2. Monthly Bookings Chart -------
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var bookingsByMonth = bookings
                .GroupBy(b => b.CheckInDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.BookingMonths = bookingsByMonth.Select(x => x.Month).ToArray();
            ViewBag.BookingCounts = bookingsByMonth.Select(x => x.Count).ToArray();

            // ------- 3. Monthly Revenue Chart -------
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var revenueByMonth = payments
                .GroupBy(p => p.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.RevenueMonths = revenueByMonth.Select(x => x.Month).ToArray();
            ViewBag.RevenueAmounts = revenueByMonth.Select(x => x.Total).ToArray();

            return View();
        }
    }
}

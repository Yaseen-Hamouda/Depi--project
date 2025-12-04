using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Payment/Checkout/5
        public async Task<IActionResult> Checkout(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }


        // POST: Payment/Confirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int bookingId, string paymentMethod)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            // لو كاش ↓
            if (paymentMethod == "Cash")
            {
                booking.PaymentStatus = "Paid";

                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = booking.TotalPrice,
                    StripeTransactionId = null,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Success");
            }

            // لو Stripe (اختياري)
            if (paymentMethod == "Stripe")
            {
                return RedirectToAction("StripeCheckout", new { id = bookingId });
            }

            return View("Checkout", booking);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

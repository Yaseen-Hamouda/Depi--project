using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return View(payments);
        }

        // GET: Admin/Payments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Bookings = new SelectList(await _unitOfWork.Bookings.GetAllAsync(), "Id", "Id");
            return View();
        }


        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Bookings = new SelectList(await _unitOfWork.Bookings.GetAllAsync(), "Id", "Id");
                return View(model);
            }

            model.CreatedAt = DateTime.Now; // لو مش عايز تاخد من الفورم

            await _unitOfWork.Payments.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            ViewBag.Bookings = new SelectList(await _unitOfWork.Bookings.GetAllAsync(), "Id", "Id");

            return View(payment);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Bookings = new SelectList(await _unitOfWork.Bookings.GetAllAsync(), "Id", "Id");
                return View(model);
            }

            _unitOfWork.Payments.Update(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            return View(payment);
        }

        // POST: Delete Confirm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            _unitOfWork.Payments.Remove(payment);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

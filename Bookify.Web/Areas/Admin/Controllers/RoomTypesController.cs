using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/RoomTypes
        public async Task<IActionResult> Index()
        {
            var types = await _unitOfWork.RoomTypes.GetAllAsync();
            return View(types);
        }

        // GET: Admin/RoomTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/RoomTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _unitOfWork.RoomTypes.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            // مهم جداً
            return RedirectToAction("Index", "RoomTypes", new { area = "Admin" });
        }

        // GET: Admin/RoomTypes/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                return NotFound();

            return View(roomType);
        }

        // POST: Admin/RoomTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _unitOfWork.RoomTypes.Update(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index", "RoomTypes", new { area = "Admin" });
        }

        // GET: Admin/RoomTypes/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                return NotFound();

            return View(roomType);
        }

        // POST: Admin/RoomTypes/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                return NotFound();

            _unitOfWork.RoomTypes.Remove(roomType);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index", "RoomTypes", new { area = "Admin" });
        }
    }
}

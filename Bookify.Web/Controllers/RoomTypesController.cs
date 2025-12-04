using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RoomTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // GET: RoomTypes/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: RoomTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomType roomType)
        {
            if (!ModelState.IsValid)
            {
                return View(roomType);
            }

            await _unitOfWork.RoomTypes.AddAsync(roomType);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Room Type created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: RoomTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
            if (roomType == null)
                return NotFound();

            return View(roomType);
        }


        // POST: RoomTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomType model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            _unitOfWork.RoomTypes.Update(model);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Room Type updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: RoomTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);

            if (roomType == null)
                return NotFound();

            return View(roomType);
        }
        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);

            if (roomType == null)
                return NotFound();

            _unitOfWork.RoomTypes.Remove(roomType);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Room Type deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        // GET: RoomTypes
        public async Task<IActionResult> Index()
        {
            var data = await _unitOfWork.RoomTypes.GetAllAsync();
            return View(data);
        }
    }
}

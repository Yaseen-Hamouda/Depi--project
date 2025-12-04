using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public RoomsController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        // GET: Admin/Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();
            return View(rooms);
        }

        // GET: Admin/Rooms/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.RoomTypes = new SelectList(await _unitOfWork.RoomTypes.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoomTypes = new SelectList(await _unitOfWork.RoomTypes.GetAllAsync(), "Id", "Name");
                return View(model);
            }

            // رفع الصورة لو موجودة
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "images/rooms");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string finalPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(finalPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/images/rooms/" + fileName;
            }

            await _unitOfWork.Rooms.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);

            if (room == null)
                return NotFound();

            ViewBag.RoomTypes = new SelectList(await _unitOfWork.RoomTypes.GetAllAsync(), "Id", "Name");
            return View(room);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Room model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoomTypes = new SelectList(await _unitOfWork.RoomTypes.GetAllAsync(), "Id", "Name");
                return View(model);
            }

            var existingRoom = await _unitOfWork.Rooms.GetByIdAsync(model.Id);
            if (existingRoom == null)
                return NotFound();

            // رفع صورة جديدة لو المستخدم رفع واحدة
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "images/rooms");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string finalPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(finalPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // حذف الصورة القديمة لو موجودة
                if (!string.IsNullOrEmpty(existingRoom.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, existingRoom.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                existingRoom.ImageUrl = "/images/rooms/" + fileName;
            }

            // تحديث باقي البيانات
            existingRoom.RoomNumber = model.RoomNumber;
            //existingRoom. = model.Description;
            //existingRoom.BasePrice = model.BasePrice;
            existingRoom.RoomTypeId = model.RoomTypeId;

            _unitOfWork.Rooms.Update(existingRoom);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Delete Confirm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
                return NotFound();

            // حذف الصورة من السيرفر
            if (!string.IsNullOrEmpty(room.ImageUrl))
            {
                string imagePath = Path.Combine(_env.WebRootPath, room.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _unitOfWork.Rooms.Remove(room);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

using Bookify.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class RoomsController : Controller
{
    private readonly IUnitOfWork _unit;

    public RoomsController(IUnitOfWork unit)
    {
        _unit = unit;
    }

    public async Task<IActionResult> Index(string search, int? type)
    {
        var rooms = await _unit.Rooms.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
            rooms = rooms.Where(r => r.RoomNumber.Contains(search)).ToList();

        if (type.HasValue)
            rooms = rooms.Where(r => r.RoomTypeId == type.Value).ToList();

        ViewBag.RoomTypes = await _unit.RoomTypes.GetAllAsync();

        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _unit.Rooms.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        return View(room);
    }
}

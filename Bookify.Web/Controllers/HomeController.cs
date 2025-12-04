using Bookify.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _unitOfWork.Rooms.GetAllAsync();

        return View(rooms);   // ? ??? ????
    }
}

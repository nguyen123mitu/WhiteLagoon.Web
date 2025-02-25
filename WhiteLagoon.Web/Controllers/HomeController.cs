using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModel;

namespace WhiteLagoon.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ;
    }

    public IActionResult Index()
    {
        HomeVM homeVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
            Nights = 1,
            CheckInDate= DateOnly.FromDateTime(DateTime.Now),
        };
        return View(homeVM);
    }
    [HttpPost]
    public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
    {
        Thread.Sleep(2000);
        var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
        foreach (var villa in villaList)
        {
            if (villa.Id %2 == 0)
            {
                villa.IsAvailable = false;
            }
        }
        HomeVM homeVM = new()
        {
            VillaList = villaList,
            Nights = nights,
            CheckInDate = checkInDate,
        };
        return PartialView("_VillaList" ,homeVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

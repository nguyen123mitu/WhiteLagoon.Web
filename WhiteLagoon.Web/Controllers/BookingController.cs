using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ;
    }

    public IActionResult FinalizeBooking(int VillaId, DateOnly CheckInDate, int nights)
    {
        Booking booking = new()
        {
            VillaId = VillaId,
            Villa = _unitOfWork.Villa.Get(u => u.Id == VillaId, includeProperties: "VillaAmenity"),
            CheckInDate = CheckInDate,
            Nights = nights,
            CheckOutDate = CheckInDate.AddDays(nights),
        };
        return View(booking);
    }
    
}

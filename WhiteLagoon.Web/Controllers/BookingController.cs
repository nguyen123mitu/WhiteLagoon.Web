using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ;
    }

    [Authorize]
    public IActionResult FinalizeBooking(int VillaId, DateOnly CheckInDate, int nights)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ApplicationUser user = _unitOfWork.User.Get(u => u.Id == userId);
        Booking booking = new()
        {
            VillaId = VillaId,
            Villa = _unitOfWork.Villa.Get(u => u.Id == VillaId, includeProperties: "VillaAmenity"),
            CheckInDate = CheckInDate,
            Nights = nights,
            CheckOutDate = CheckInDate.AddDays(nights),
            UserId = userId,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Name = user.Name,
        };
        booking.TotalCost = booking.Villa.Price * nights;
        return View(booking);
    }
    
    
    [Authorize]
    [HttpPost]
    public IActionResult FinalizeBooking(Booking booking)
    {
        var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);
        booking.TotalCost = villa.Price * booking.Nights;
        booking.Status = SD.StatusPending;
        booking.BookingDate = DateTime.Now;

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + "Booking/BookingConfirmation?BookingId=" + booking.Id,
            CancelUrl = domain + "Booking/FinalizeBooking?VillaId=" + booking.VillaId + "&CheckInDate=" + booking.CheckInDate + "&nights=" + booking.Nights,
        };

        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = villa.Name,
                    // Images = new List<string> { domain + villa.ImageUrl },
                },
            },
            Quantity = 1,
        });

        var service = new Stripe.Checkout.SessionService();
        Stripe.Checkout.Session session = service.Create(options);

        _unitOfWork.Booking.UpdateStripePaymentId(booking.Id ,session.Id ,session.PaymentIntentId);  
        _unitOfWork.Save(); 
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    [Authorize]
    public IActionResult BookingConfirmation(int BookingId)
    {
        Booking bookingFormDb = _unitOfWork.Booking.Get(u => u.Id == BookingId, includeProperties: "User,Villa");
        if(bookingFormDb.Status == SD.StatusPending)
        {
            // this is pending order , we need to confirm the payment was successful
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(bookingFormDb.StripeSessionId);
            if (session.PaymentStatus == "paid")
            {
                _unitOfWork.Booking.UpdateStatus(bookingFormDb.Id, SD.StatusApproved);
                _unitOfWork.Booking.UpdateStripePaymentId(bookingFormDb.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }
        }
        return View(BookingId);
    }
}

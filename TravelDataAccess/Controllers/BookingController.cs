using Microsoft.AspNetCore.Mvc;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Controllers;

public class BookingController(IBookingService bookingService, ITripService tripService) : Controller
{
    // GET: Booking/MyBookings - View logged-in user's bookings
    public async Task<IActionResult> MyBookings()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to view your bookings";
            return RedirectToAction("Login", "Customer");
        }

        var bookings = await bookingService.GetAllBookingsByCustomerId(customerId.Value);
        
        // Load trip details for each booking
        var bookingList = bookings.ToList();
        foreach (var booking in bookingList)
        {
            booking.Trip = await tripService.GetTripById(booking.TripId);
        }

        return View(bookingList);
    }

    // GET: Booking/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to view booking details";
            return RedirectToAction("Login", "Customer");
        }

        if (id == null)
        {
            return NotFound();
        }

        var bookings = await bookingService.GetAllBookingsByCustomerId(customerId.Value);
        var booking = bookings.FirstOrDefault(b => b.BookingId == id.Value);

        if (booking == null)
        {
            return NotFound();
        }

        // Load trip details
        booking.Trip = await tripService.GetTripById(booking.TripId);

        return View(booking);
    }

    // GET: Booking/Create - Create a new booking
    public async Task<IActionResult> Create(int? tripId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to create a booking";
            return RedirectToAction("Login", "Customer");
        }

        if (tripId != null)
        {
            var trip = await tripService.GetTripById(tripId.Value);
            if (trip != null)
            {
                ViewBag.SelectedTrip = trip;
            }
        }

        var trips = await tripService.GetAllTrips();
        ViewBag.AvailableTrips = trips.Where(t => t.Status == "Available").ToList();

        return View();
    }

    // POST: Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int tripId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to create a booking";
            return RedirectToAction("Login", "Customer");
        }

        var trip = await tripService.GetTripById(tripId);
        if (trip == null)
        {
            ModelState.AddModelError("", "Trip not found");
            return View();
        }

        if (trip.Status != "Available")
        {
            ModelState.AddModelError("", "This trip is not available for booking");
            return View();
        }

        var booking = new Booking
        {
            CustomerId = customerId.Value,
            TripId = tripId,
            BookingDate = DateOnly.FromDateTime(DateTime.Now),
            Status = "Confirmed"
        };

        var result = await bookingService.CreateBooking(booking);

        if (result != null)
        {
            TempData["Success"] = "Booking created successfully!";
            return RedirectToAction(nameof(MyBookings));
        }

        ModelState.AddModelError("", "Failed to create booking");
        return View();
    }

    // GET: Booking/Cancel/5
    public async Task<IActionResult> Cancel(int? id)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to cancel bookings";
            return RedirectToAction("Login", "Customer");
        }

        if (id == null)
        {
            return NotFound();
        }

        var bookings = await bookingService.GetAllBookingsByCustomerId(customerId.Value);
        var booking = bookings.FirstOrDefault(b => b.BookingId == id.Value);

        if (booking == null)
        {
            return NotFound();
        }

        // Load trip details
        booking.Trip = await tripService.GetTripById(booking.TripId);

        return View(booking);
    }

    // POST: Booking/Cancel/5
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["Error"] = "Please login to cancel bookings";
            return RedirectToAction("Login", "Customer");
        }

        // Verify the booking belongs to the logged-in customer
        var bookings = await bookingService.GetAllBookingsByCustomerId(customerId.Value);
        var booking = bookings.FirstOrDefault(b => b.BookingId == id);

        if (booking == null)
        {
            TempData["Error"] = "Booking not found or you don't have permission to cancel it";
            return RedirectToAction(nameof(MyBookings));
        }

        // Use the CancelBooking service method which handles both booking and trip status updates
        var (success, message) = await bookingService.CancelBooking(id);

        if (success)
        {
            TempData["Success"] = message;
        }
        else
        {
            TempData["Error"] = message;
        }

        return RedirectToAction(nameof(MyBookings));
    }
}


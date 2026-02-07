using Microsoft.AspNetCore.Mvc;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Controllers;

public class TripController(ITripService tripService) : Controller
{
    // GET: Trip
    public async Task<IActionResult> Index()
    {
        var trips = await tripService.GetAllTrips();
        return View(trips);
    }

    // GET: Trip/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trip = await tripService.GetTripById(id.Value);
        if (trip == null)
        {
            return NotFound();
        }

        return View(trip);
    }

    // GET: Trip/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Trip/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TripId,Code,Destination,Price,Status")] Trip trip)
    {
        if (ModelState.IsValid)
        {
            var result = await tripService.CreateTrip(trip);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create trip. Code may already exist.");
        }
        return View(trip);
    }

    // GET: Trip/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trip = await tripService.GetTripById(id.Value);
        if (trip == null)
        {
            return NotFound();
        }
        return View(trip);
    }

    // POST: Trip/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TripId,Code,Destination,Price,Status")] Trip trip)
    {
        if (id != trip.TripId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var (success, updatedTrip) = await tripService.UpdateTrip(trip);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to update trip. Code may already exist or trip not found.");
        }
        return View(trip);
    }

    // GET: Trip/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trip = await tripService.GetTripById(id.Value);
        if (trip == null)
        {
            return NotFound();
        }

        return View(trip);
    }

    // POST: Trip/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await tripService.DeleteTrip(id);
        if (!result)
        {
            TempData["Error"] = "Cannot delete trip. It may have existing bookings or doesn't exist.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }
}


using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Services.Implements;

public class TripService(DbtravelCenterContext context) : ITripService
{
    public async Task<IEnumerable<Trip>> GetAllTrips()
    {
        return await context.Trips.ToListAsync();
    }

    public async Task<Trip?> GetTripById(int id)
    {
        return await context.Trips.FindAsync(id);
    }

    public async Task<Trip?> GetTripByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await context.Trips.FirstOrDefaultAsync(t => t.Code == code);
    }

    public async Task<Trip?> CreateTrip(Trip trip)
    {
        if (string.IsNullOrWhiteSpace(trip.Code) || 
            string.IsNullOrWhiteSpace(trip.Destination) ||
            trip.Price <= 0 || 
            string.IsNullOrWhiteSpace(trip.Status))
        {
            return null;
        }

        // Check if trip code already exists
        if (await CheckTripCodeExist(trip.Code))
        {
            return null;
        }

        await context.Trips.AddAsync(trip);
        await context.SaveChangesAsync();

        return trip;
    }

    public async Task<(bool Success, Trip? trip)> UpdateTrip(Trip trip)
    {
        if (string.IsNullOrWhiteSpace(trip.Code) || 
            string.IsNullOrWhiteSpace(trip.Destination) ||
            trip.Price <= 0 || 
            string.IsNullOrWhiteSpace(trip.Status))
        {
            return (false, null);
        }

        var existingTrip = await context.Trips.FindAsync(trip.TripId);
        if (existingTrip == null)
        {
            return (false, null);
        }

        // Check if the new code is already used by another trip
        if (existingTrip.Code != trip.Code && await CheckTripCodeExist(trip.Code))
        {
            return (false, null);
        }

        existingTrip.Code = trip.Code;
        existingTrip.Destination = trip.Destination;
        existingTrip.Price = trip.Price;
        existingTrip.Status = trip.Status;

        context.Trips.Update(existingTrip);
        await context.SaveChangesAsync();

        return (true, existingTrip);
    }

    public async Task<bool> DeleteTrip(int id)
    {
        var trip = await context.Trips.FindAsync(id);
        if (trip == null)
        {
            return false;
        }

        // Check if trip has bookings
        var hasBookings = await context.Bookings.AnyAsync(b => b.TripId == id);
        if (hasBookings)
        {
            return false; // Cannot delete trip with existing bookings
        }

        context.Trips.Remove(trip);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckTripCodeExist(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return await context.Trips.AnyAsync(t => t.Code == code);
    }
}
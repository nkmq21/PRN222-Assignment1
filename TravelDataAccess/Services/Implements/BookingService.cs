using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Services.Implements;

public class BookingService(DbtravelCenterContext context) : IBookingService
{

    public async Task<IEnumerable<Booking>> GetAllBookingsByCustomerId(int id)
    {
        return await context.Bookings.Where(b => b.CustomerId == id).ToListAsync();
    }

    public async Task<(bool Success, Booking? booking)> UpdateBooking(Booking booking)
    {
        if (string.IsNullOrEmpty(booking.CustomerId.ToString()) ||
            string.IsNullOrEmpty(booking.TripId.ToString()) ||
            string.IsNullOrWhiteSpace(booking.BookingDate.ToString()))
        {
            return (false, null);
        }

        context.Bookings.Update(booking);
        await context.SaveChangesAsync();

        return (true, booking);
    }

    public async Task<Booking?> CreateBooking(Booking booking)
    {
        if (string.IsNullOrEmpty(booking.CustomerId.ToString()) ||
            string.IsNullOrEmpty(booking.TripId.ToString()) ||
            string.IsNullOrWhiteSpace(booking.BookingDate.ToString()))
        {
            return null;
        }

        var trip = await context.Trips.FindAsync(booking.TripId);
        if (trip == null)
        {
            return null;
        }

        if (trip.Status.Equals("Available"))
        {
            // Add the booking
            await context.Bookings.AddAsync(booking);
            
            // Update trip status to "Booked"
            trip.Status = "Booked";
            context.Trips.Update(trip);
            
            await context.SaveChangesAsync();
            
            return booking;
        }

        return null;
    }

    public async Task<(bool Success, string Message)> CancelBooking(int bookingId)
    {
        var booking = await context.Bookings.FindAsync(bookingId);
        if (booking == null)
        {
            return (false, "Booking not found");
        }

        if (booking.Status == "Cancelled")
        {
            return (false, "Booking is already cancelled");
        }

        // Update booking status to Cancelled
        booking.Status = "Cancelled";
        context.Bookings.Update(booking);

        // Get the trip and update its status back to Available
        var trip = await context.Trips.FindAsync(booking.TripId);
        if (trip != null && trip.Status == "Booked")
        {
            trip.Status = "Available";
            context.Trips.Update(trip);
        }

        await context.SaveChangesAsync();

        return (true, "Booking cancelled successfully");
    }
}
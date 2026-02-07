namespace TravelDataAccess.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsByCustomerId(int id);
    Task<(bool Success, Booking? booking)> UpdateBooking(Booking booking);
    Task<Booking?> CreateBooking(Booking booking);
    Task<(bool Success, string Message)> CancelBooking(int bookingId);
}
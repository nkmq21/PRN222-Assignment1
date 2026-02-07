namespace TravelDataAccess.Services.Interfaces;

public interface ITripService
{
    Task<IEnumerable<Trip>> GetAllTrips();
    Task<Trip?> GetTripById(int id);
    Task<Trip?> GetTripByCode(string code);
    Task<Trip?> CreateTrip(Trip trip);
    Task<(bool Success, Trip? trip)> UpdateTrip(Trip trip);
    Task<bool> DeleteTrip(int id);
    Task<bool> CheckTripCodeExist(string code);
}
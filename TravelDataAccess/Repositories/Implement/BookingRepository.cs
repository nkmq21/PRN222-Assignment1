using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Repositories.Interface;

namespace TravelDataAccess.Repositories.Implement;

public class BookingRepository : IBookingRepository
{
    private readonly DbtravelCenterContext _context;

    public BookingRepository(DbtravelCenterContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync<T>(object id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
    }

    public async Task DeleteAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
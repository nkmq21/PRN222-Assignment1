using TravelDataAccess.Repositories.Interface;

namespace TravelDataAccess.Repositories.Implement;

public class TripRepository : ICustomerRepository
{
    public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync<T>(object id) where T : class
    {
        throw new NotImplementedException();
    }

    public Task AddAsync<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
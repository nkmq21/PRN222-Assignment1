namespace TravelDataAccess.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<(bool Success, string Messsage, Customer? customer)> LoginAsync(object credential, object password);
    Task<(bool Success, string Message)> RegisterCustomer(Customer customer);
    Task<(bool Success, string Message, Customer? customer)> AddCustomerAsync(Customer customer);
    Task<(bool Success, string Message, Customer? customer)> UpdateCustomerAsync(Customer customer);
    Task<(bool Success, string Message)> DeleteCustomer(int id);
    Task<bool> CheckEmailExist(string email);
    Task<bool> CheckCustomerCodeExist(string code);
}
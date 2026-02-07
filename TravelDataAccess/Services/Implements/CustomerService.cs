using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Services.Implements;

public class CustomerService : ICustomerService
{
    private readonly DbtravelCenterContext _context;

    public CustomerService(DbtravelCenterContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<(bool Success, string Messsage, Customer? customer)> LoginAsync(object credential, object password)
    {
        string customerCode = credential?.ToString()?.Trim();
        string customerPassword = password?.ToString();

        if (string.IsNullOrWhiteSpace(customerCode) || string.IsNullOrEmpty(customerPassword))
        {
            return (false, "Invalid credentials", null);
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Code == customerCode);

        if (customer == null)
        {
            return (false, "Invalid credentials", null);
        }

        if (!string.Equals(customer.Password, customerPassword))
        {
            return (false, "Invalid credentials", null);
        }

        return (true, "Login Successful", customer);
    }

    public async Task<(bool Success, string Message)> RegisterCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.FullName) ||
            string.IsNullOrEmpty(customer.Password))
        {
            return (false, "Invalid information");
        }

        if (customer.Email != null && await CheckEmailExist(customer.Email))
        {
            return (false, "Duplicate email");
        }

        if (await CheckCustomerCodeExist(customer.Code))
        {
            return (false, "Duplicate code");
        }

        var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
        if (existingCustomer == null)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }
        else
        {
            return (false, "Customer already exist");
        }

        return (true, "Register successful!");
    }

    public async Task<(bool Success, string Message, Customer? customer)> AddCustomerAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.FullName) ||
            string.IsNullOrEmpty(customer.Password))
        {
            return (false, "Invalid information", null);
        }

        // Check if customer code already exists
        if (await CheckCustomerCodeExist(customer.Code))
        {
            return (false, "Customer code already exists", null);
        }

        // Check if email already exists
        if (!string.IsNullOrWhiteSpace(customer.Email) && await CheckEmailExist(customer.Email))
        {
            return (false, "Email already exists", null);
        }

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return (true, "Customer created successfully", customer);
    }

    public async Task<(bool Success, string Message, Customer? customer)> UpdateCustomerAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.FullName) ||
            string.IsNullOrEmpty(customer.Password))
        {
            return (false, "Invalid information", null);
        }

        var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
        if (existingCustomer == null)
        {
            return (false, "Customer not found", null);
        }

        // Check if the new code is already used by another customer
        if (existingCustomer.Code != customer.Code && await CheckCustomerCodeExist(customer.Code))
        {
            return (false, "Customer code already exists", null);
        }

        // Check if the new email is already used by another customer
        if (!string.IsNullOrWhiteSpace(customer.Email) && 
            existingCustomer.Email != customer.Email && 
            await CheckEmailExist(customer.Email))
        {
            return (false, "Email already exists", null);
        }

        existingCustomer.Code = customer.Code;
        existingCustomer.FullName = customer.FullName;
        existingCustomer.Email = customer.Email;
        existingCustomer.Age = customer.Age;
        existingCustomer.Password = customer.Password;

        _context.Customers.Update(existingCustomer);
        await _context.SaveChangesAsync();

        return (true, "Customer updated successfully", existingCustomer);
    }

    public async Task<(bool Success, string Message)> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return (false, "Customer not found");
        }

        // Check if customer has bookings
        var hasBookings = await _context.Bookings.AnyAsync(b => b.CustomerId == id);
        if (hasBookings)
        {
            return (false, "Cannot delete customer with existing bookings");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return (true, "Customer deleted successfully");
    }

    public async Task<bool> CheckEmailExist(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }
        return await _context.Customers.AnyAsync(c => c.Email == email);
    }

    public async Task<bool> CheckCustomerCodeExist(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return false;
        }
        return await _context.Customers.AnyAsync(c => c.Code == code);
    }
}
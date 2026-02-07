using Microsoft.AspNetCore.Mvc;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess.Controllers;

public class CustomerController(ICustomerService customerService, IBookingService bookingService) : Controller
{
    // GET: Customer/Index - List all customers (Admin view)
    public async Task<IActionResult> Index()
    {
        var customers = await customerService.GetAllCustomersAsync();
        return View(customers);
    }

    // GET: Customer/Login
    public IActionResult Login()
    {
        // If already logged in, redirect to profile
        if (HttpContext.Session.GetInt32("CustomerId") != null)
        {
            return RedirectToAction(nameof(Profile));
        }
        return View();
    }

    // POST: Customer/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string code, string password)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Please enter both code and password");
            return View();
        }

        var (success, message, customer) = await customerService.LoginAsync(code, password);

        if (success && customer != null)
        {
            // Store customer info in session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName);
            HttpContext.Session.SetString("CustomerCode", customer.Code);

            TempData["Success"] = message;
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", message);
        return View();
    }

    // GET: Customer/Register
    public IActionResult Register()
    {
        // If already logged in, redirect to profile
        if (HttpContext.Session.GetInt32("CustomerId") != null)
        {
            return RedirectToAction(nameof(Profile));
        }
        return View();
    }

    // POST: Customer/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Code,FullName,Email,Age,Password")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            var (success, message) = await customerService.RegisterCustomer(customer);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", message);
        }
        return View(customer);
    }

    // GET: Customer/Profile
    public async Task<IActionResult> Profile()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var customer = await customerService.GetCustomerByIdAsync(customerId.Value);
        if (customer == null)
        {
            return RedirectToAction(nameof(Logout));
        }

        // Get customer's bookings
        var bookings = await bookingService.GetAllBookingsByCustomerId(customerId.Value);
        ViewBag.Bookings = bookings;

        return View(customer);
    }

    // GET: Customer/Edit
    public async Task<IActionResult> Edit()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var customer = await customerService.GetCustomerByIdAsync(customerId.Value);
        if (customer == null)
        {
            return RedirectToAction(nameof(Logout));
        }

        return View(customer);
    }

    // POST: Customer/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("CustomerId,Code,FullName,Email,Age,Password")] Customer customer)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null || customerId.Value != customer.CustomerId)
        {
            return RedirectToAction(nameof(Login));
        }

        if (ModelState.IsValid)
        {
            var (success, message, updatedCustomer) = await customerService.UpdateCustomerAsync(customer);

            if (success && updatedCustomer != null)
            {
                // Update session data
                HttpContext.Session.SetString("CustomerName", updatedCustomer.FullName);
                HttpContext.Session.SetString("CustomerCode", updatedCustomer.Code);

                TempData["Success"] = message;
                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError("", message);
        }
        return View(customer);
    }

    // GET: Customer/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out successfully";
        return RedirectToAction("Index", "Home");
    }

    // GET: Customer/Details/5 (Admin view)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await customerService.GetCustomerByIdAsync(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        // Get customer's bookings
        var bookings = await bookingService.GetAllBookingsByCustomerId(id.Value);
        ViewBag.Bookings = bookings;

        return View(customer);
    }

    // GET: Customer/Create (Admin view)
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customer/Create (Admin view)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Code,FullName,Email,Age,Password")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            var (success, message, newCustomer) = await customerService.AddCustomerAsync(customer);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
        }
        return View(customer);
    }

    // GET: Customer/Delete/5 (Admin view)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await customerService.GetCustomerByIdAsync(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: Customer/Delete/5 (Admin view)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (success, message) = await customerService.DeleteCustomer(id);

        if (success)
        {
            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = message;
        return RedirectToAction(nameof(Delete), new { id });
    }
}
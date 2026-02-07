using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Services.Implements;
using TravelDataAccess.Services.Interfaces;

namespace TravelDataAccess;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add session support
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddDbContext<DbtravelCenterContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DBTravelCenter"));
        });
        
        // Register services
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Enable session middleware
        app.UseSession();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
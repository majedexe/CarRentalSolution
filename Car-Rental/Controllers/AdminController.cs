using Car_Rental.Data;
using Car_Rental.Models.Enums;
using Car_Rental.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context,IWebHostEnvironment environment,ILogger<AdminController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var totalCars = await _context.Cars.CountAsync();
            var availableCars = await _context.Cars.CountAsync(c => c.Status == CarStatus.Available);
            var rentedCars = await _context.Cars.CountAsync(c => c.Status == CarStatus.Rented);
            var maintenanceCars = await _context.Cars.CountAsync(c => c.Status == CarStatus.Maintenance);

            var activeRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Active);
            var completedRentals = await _context.Rentals.CountAsync(r => r.Status == RentalStatus.Completed);

            var totalRevenue = await _context.Rentals
                .Where(r => r.Status == RentalStatus.Completed)
                .SumAsync(r => (decimal?)r.TotalPrice) ?? 0m;

            var vm = new DashboardViewModel
            {
                TotalCars = totalCars,
                AvailableCars = availableCars,
                RentedCars = rentedCars,
                MaintenanceCars = maintenanceCars,
                ActiveRentals = activeRentals,
                CompletedRentals = completedRentals,
                TotalRevenue = totalRevenue
            };
            try
            {
                var logsFolder = Path.Combine(_environment.ContentRootPath, "Logs");
                if (Directory.Exists(logsFolder))
                {
                    var latestLogFile = Directory
                        .GetFiles(logsFolder, "log-*.txt")
                        .OrderByDescending(f => f)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(latestLogFile))
                    {
                        var allLines = System.IO.File.ReadAllLines(latestLogFile);
                        vm.RecentLogLines = allLines
                            .Reverse()
                            .Take(10)
                            .Reverse()
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read recent log lines for admin dashboard.");
            }
            return View(vm);
        }
    }
}

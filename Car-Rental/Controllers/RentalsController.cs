using Car_Rental.Data;
using Car_Rental.Models;
using Car_Rental.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental.Controllers
{

    [Authorize]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RentalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Staff"))
            {
                var allRentals = await _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return View(allRentals);
            }
            else
            {
                var userId = _userManager.GetUserId(User);

                var myRentals = await _context.Rentals
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Car)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return View(myRentals);
            }
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null) return NotFound();

            // Normal users can only see their own rentals
            if (!User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                var userId = _userManager.GetUserId(User);
                if (rental.UserId != userId)
                {
                    return Forbid();
                }
            }

            return View(rental);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null || car.Status != CarStatus.Available)
            {
                return NotFound();
            }

            var model = new Rental
            {
                CarId = car.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            ViewBag.Car = car;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rental rental)
        {
            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car == null)
            {
                ModelState.AddModelError(string.Empty, "Selected car does not exist.");
                return View(rental);
            }

            if (car.Status != CarStatus.Available)
            {
                ModelState.AddModelError(string.Empty, "This car is not available for rental.");
            }

            if (rental.EndDate <= rental.StartDate)
            {
                ModelState.AddModelError(nameof(Rental.EndDate), "End date must be after start date.");
            }

            bool overlaps = await _context.Rentals.AnyAsync(r =>
                r.CarId == rental.CarId &&
                (r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending) &&
                r.StartDate < rental.EndDate &&
                r.EndDate > rental.StartDate);

            if (overlaps)
            {
                ModelState.AddModelError(string.Empty, "This car is already booked for the selected dates.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Car = car;
                return View(rental);
            }

            var userId = _userManager.GetUserId(User);
            rental.UserId = userId!;
            rental.Status = RentalStatus.Active;
            rental.CreatedAt = DateTime.UtcNow;

            int totalDays = (int)(rental.EndDate.Date - rental.StartDate.Date).TotalDays;
            if (totalDays <= 0) totalDays = 1;
            rental.TotalPrice = totalDays * car.DailyRate;

            _context.Rentals.Add(rental);

            // Mark car as rented
            car.Status = CarStatus.Rented;
            _context.Cars.Update(car);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = rental.Id });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> Return(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null) return NotFound();

            if (rental.Status != RentalStatus.Active)
            {
                return BadRequest("Only active rentals can be returned.");
            }

            return View(rental);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, CarStatus carStatusAfterReturn)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null) return NotFound();

            if (rental.Status != RentalStatus.Active)
            {
                return BadRequest("Only active rentals can be returned.");
            }

            rental.Status = RentalStatus.Completed;

            
            if (rental.EndDate > DateTime.Today)
            {
                rental.EndDate = DateTime.Today;
                int totalDays = (int)(rental.EndDate.Date - rental.StartDate.Date).TotalDays;
                if (totalDays <= 0) totalDays = 1;
                rental.TotalPrice = totalDays * rental.Car.DailyRate;
            }

            var car = rental.Car;
            car.Status = carStatusAfterReturn;

            _context.Rentals.Update(rental);
            _context.Cars.Update(car);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = rental.Id });
        }
    }
}


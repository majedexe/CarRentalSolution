using Car_Rental.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Car_Rental.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Car")]
        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; } = string.Empty;

        public IdentityUser User { get; set; } = null!;

        [Required(ErrorMessage = "Start date is required.")]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Total price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public RentalStatus Status { get; set; } = RentalStatus.Active;

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date.",
                    new[] { nameof(EndDate) });
            }
        }
    }
}

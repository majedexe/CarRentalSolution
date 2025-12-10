using Car_Rental.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
        public string Model { get; set; } = string.Empty;

        [Range(1980, 2040, ErrorMessage = "Year must be between 1980 and 2100.")]
        public int Year { get; set; }

        [Range(0, 10000, ErrorMessage = "Daily rate must be between 0 and 10,000.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Daily rate")]
        public decimal DailyRate { get; set; }

        [Required]
        [Display(Name = "Category")]
        public CarCategory Category { get; set; }

        [Required]
        [Display(Name = "Status")]
        public CarStatus Status { get; set; } = CarStatus.Available;

        [Display(Name = "Image file name")]
        public string? ImageFileName { get; set; }

    }
}


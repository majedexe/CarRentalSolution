using Car_Rental.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1970, 2040)]
        public int Year { get; set; }

        [Range(0, 10000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Daily rate")]
        public decimal DailyRate { get; set; }

        [Display(Name = "Category")]
        public CarCategory Category { get; set; }

        [Display(Name = "Status")]
        public CarStatus Status { get; set; } = CarStatus.Available;

        [Display(Name = "Image file name")]
        public string? ImageFileName { get; set; }

    }
}


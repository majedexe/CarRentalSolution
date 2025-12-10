using Car_Rental.Models.Enums;

namespace Car_Rental.Models.ViewModels
{
    public class CarIndexViewModel
    {
        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
        public string? SearchString { get; set; }
        public CarCategory? Category { get; set; }
        public CarStatus? Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages =>
            PageSize == 0 ? 0 : (int)System.Math.Ceiling((double)TotalItems / PageSize);
    }
}

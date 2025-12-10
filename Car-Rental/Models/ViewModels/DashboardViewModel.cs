namespace Car_Rental.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int RentedCars { get; set; }
        public int MaintenanceCars { get; set; }

        public int ActiveRentals { get; set; }
        public int CompletedRentals { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}

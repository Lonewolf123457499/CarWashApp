using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vehicle ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Vehicle ID must be greater than 0")]
        public int VehicleId { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Wash package ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Wash package ID must be greater than 0")]
        public int WashPackageId { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public string? Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total price must be non-negative")]
        public decimal TotalPrice { get; set; }

        public List<int>? AddonIds { get; set; }
    }
}

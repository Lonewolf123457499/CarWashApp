using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTO
{
    public class VehicleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vehicle make is required")]
        [StringLength(50, ErrorMessage = "Make cannot exceed 50 characters")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Vehicle model is required")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "License plate is required")]
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Number plate is required")]
        [StringLength(20, ErrorMessage = "Number plate cannot exceed 20 characters")]
        public string NumberPlate { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ImageUrl { get; set; }
    }
}

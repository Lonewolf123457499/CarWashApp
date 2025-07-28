using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTO
{
    public class PaymentDTO
    {
        [Required(ErrorMessage = "Order ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be greater than 0")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Payment nonce is required")]
        public string Nonce { get; set; }
    }
}

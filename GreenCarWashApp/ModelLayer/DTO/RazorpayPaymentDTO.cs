namespace ModelLayer.DTO
{
    public class RazorpayPaymentDTO
    {
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string Signature { get; set; }
    }
}
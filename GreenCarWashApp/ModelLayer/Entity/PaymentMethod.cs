public class PaymentMethod : BaseEntity
{
    public string CardType { get; set; }     // e.g., Visa
    public string CardLast4Digits { get; set; }
    public string BraintreeToken { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}

public class Receipt : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public string ReceiptNumber { get; set; }
    public string ImageUrl { get; set; }
    public string Details { get; set; }
    public decimal Total { get; set; }
}

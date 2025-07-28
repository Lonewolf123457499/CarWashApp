public class Rating : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int Stars { get; set; } // 1 to 5
    public string Comment { get; set; }

    public bool ByCustomer { get; set; } // true = customer rated washer, false = washer rated customer
}

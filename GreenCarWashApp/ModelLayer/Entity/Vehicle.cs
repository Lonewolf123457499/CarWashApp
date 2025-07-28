public class Vehicle : BaseEntity
{
    public string Make { get; set; }
    public string Model { get; set; }
    public string LicensePlate { get; set; }
    public string ImageUrl { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public ICollection<Order> Orders { get; set; }
}

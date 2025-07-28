public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int? WasherId { get; set; }
    public Washer Washer { get; set; }

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }

    public int WashPackageId { get; set; }
    public WashPackage WashPackage { get; set; }

    public DateTime ScheduledTime { get; set; }
    public DateTime? CompletedTime { get; set; }

    public string Status { get; set; } // Pending, InProgress, Completed, Cancelled
    public string ImageAfterWashUrl { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<OrderAddon> OrderAddons { get; set; }

    public Rating Rating { get; set; }
    public Receipt Receipt { get; set; }
}

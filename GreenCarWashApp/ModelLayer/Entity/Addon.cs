public class Addon : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public ICollection<OrderAddon> OrderAddons { get; set; }
}

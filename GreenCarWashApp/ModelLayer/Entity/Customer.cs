public class Customer : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public string Password { get; set; } // Plain for now; should be hashed
     public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; }
    public ICollection<PaymentMethod> PaymentMethods { get; set; }
    public ICollection<Order> Orders { get; set; }
}

public class Washer : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public string Password { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Order> Orders { get; set; }
}

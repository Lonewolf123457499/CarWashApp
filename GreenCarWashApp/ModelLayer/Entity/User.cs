public class User : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Customer", "Washer", "Admin"
    public bool IsActive { get; set; }

    public Customer? Customer { get; set; }
    public Washer? Washer { get; set; }

}
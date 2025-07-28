using GreenWashBackend.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

     public async Task AddUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
         catch (DbUpdateException ex)
    {
        throw new Exception("Failed to add user to database. Please check if all required fields are filled correctly.", ex);
    }
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        try
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
         catch (DbUpdateException ex)
    {
        throw new Exception("Failed to add user to database. Please check if all required fields are filled correctly.", ex);
    }
    }

    public async Task AddWasherAsync(Washer washer)
    {
        try
        {
            _context.Washers.Add(washer);
            await _context.SaveChangesAsync();
        }
         catch (DbUpdateException ex)
    {
        throw new Exception("Failed to add user to database. Please check if all required fields are filled correctly.", ex);
    }
    }
}

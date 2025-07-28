using ModelLayer.DTO;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task AddCustomerAsync(Customer customer);
    Task AddWasherAsync(Washer washer);
    Task<bool> EmailExistsAsync(string email);
}

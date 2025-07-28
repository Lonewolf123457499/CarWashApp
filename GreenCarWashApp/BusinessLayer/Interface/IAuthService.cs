using ModelLayer.DTO;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<object> LoginAsync(LoginRequest dto);
    Task RegisterCustomerAsync(RegisterRequest dto);
    Task RegisterWasherAsync(RegisterRequest dto);
}

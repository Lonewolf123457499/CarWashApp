using ModelLayer.DTO;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    public async Task RegisterCustomerAsync(RegisterRequest dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            throw new Exception("Email already registered.");

        var user = new User
        {
            FullName = dto.Name,
            Email = dto.Email,
            Password = dto.Password, 
            Role = "Customer",
            IsActive = true
        };
        await _userRepo.AddUserAsync(user);

        var customer = new Customer
        {
            FullName = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Phone= dto.PhoneNumber,
            UserId = user.Id,
            
        };
        await _userRepo.AddCustomerAsync(customer);
    }

    public async Task RegisterWasherAsync(RegisterRequest dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            throw new Exception("Email already registered.");

        var user = new User
        {
            FullName = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = "Washer",
            IsActive = true
        };
        await _userRepo.AddUserAsync(user);

        var washer = new Washer
        {
            FullName = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Phone= dto.PhoneNumber,
            UserId = user.Id
        };
        await _userRepo.AddWasherAsync(washer);
    }

    public async Task<object> LoginAsync(LoginRequest dto)
    {
        var user = await _userRepo.GetUserByEmailAsync(dto.Email);

        if (user == null || user.Password != dto.Password)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Role);
        
        return new {
            token = token,
            role = user.Role,
            message = "Login successful"
        };
    }
}

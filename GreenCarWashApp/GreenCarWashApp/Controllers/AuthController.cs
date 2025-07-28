using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace GreenCarWashApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterRequest dto)
        {
            await _authService.RegisterCustomerAsync(dto);
            return Ok(new { message = "Customer registered successfully." });
        }

        [HttpPost("register/washer")]
        public async Task<IActionResult> RegisterWasher([FromBody] RegisterRequest dto)
        {
            await _authService.RegisterWasherAsync(dto);
            return Ok(new { message = "Washer registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}

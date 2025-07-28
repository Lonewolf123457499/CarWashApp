using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using Microsoft.AspNetCore.Authorization;

namespace GreenCarWashApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Wash Package Management
        [HttpPost("washpackage")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddWashPackage([FromBody] AdminWashPackageDTO dto)
        {
            var result = await _adminService.AddWashPackageAsync(dto);
            return Ok(result);
        }

        [HttpGet("washpackages")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetWashPackages()
        {
            var packages = await _adminService.GetWashPackagesAsync();
            return Ok(packages);
        }

        [HttpPut("washpackage/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWashPackage(int id, [FromBody] AdminWashPackageDTO dto)
        {
            var result = await _adminService.UpdateWashPackageAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("washpackage/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteWashPackage(int id)
        {
            var result = await _adminService.DeleteWashPackageAsync(id);
            return Ok(result);
        }

        // Addon Management
        [HttpPost("addon")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAddon([FromBody] AdminAddonDTO dto)
        {
            var result = await _adminService.AddAddonAsync(dto);
            return Ok(result);
        }

        [HttpGet("addons")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAddons()
        {
            var addons = await _adminService.GetAddonsAsync();
            return Ok(addons);
        }

        [HttpPut("addon/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAddon(int id, [FromBody] AdminAddonDTO dto)
        {
            var result = await _adminService.UpdateAddonAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("addon/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAddon(int id)
        {
            var result = await _adminService.DeleteAddonAsync(id);
            return Ok(result);
        }

        // Washer Management
        [HttpGet("washers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWashers()
        {
            var washers = await _adminService.GetWashersAsync();
            return Ok(washers);
        }

        [HttpPut("washer/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWasherStatus(int id, [FromBody] WasherStatusDTO dto)
        {
            var result = await _adminService.UpdateWasherStatusAsync(id, dto);
            return Ok(result);
        }

        // Customer Management
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _adminService.GetCustomersAsync();
            return Ok(customers);
        }

        // Order Management
        [HttpGet("orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _adminService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderStats()
        {
            var stats = await _adminService.GetOrderStatsAsync();
            return Ok(stats);
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}
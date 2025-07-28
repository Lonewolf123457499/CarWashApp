using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ModelLayer.DTO;
using CarWashService.DTOs;
using BusinessLayer.Interface;

namespace CarWashService.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

       
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var profile = await _customerService.GetProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] CarWashService.DTOs.UpdateCustomerDto dto)
        {
            var userId = GetUserId();
            var result = await _customerService.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }

        

        [HttpPost("vehicle")]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDTO dto)
        {
            var userId = GetUserId();
            var result = await _customerService.AddVehicleAsync(userId, dto);
            return Ok(new { Message = result });
        }


      
        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var userId = GetUserId();
            var vehicles = await _customerService.GetVehiclesAsync(userId);
            return Ok(vehicles);
        }

        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var userId = GetUserId();
            var vehicle = await _customerService.GetVehicleAsync(userId, id);
            return Ok(vehicle);
        }

        
        [HttpDelete("vehicle/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var userId = GetUserId();
            var result = await _customerService.DeleteVehicleAsync(userId, id);
            return Ok(result);
        }

        [HttpPost("order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDTO dto)
        {
            var userId = GetUserId();
            var result = await _customerService.PlaceOrderAsync(userId, dto);
            return Ok(result);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = GetUserId();
            var orders = await _customerService.GetOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPost("rating")]
        public async Task<IActionResult> RateWasher([FromBody] OrderRatingDTO dto)
        {
            var userId = GetUserId();
            var result = await _customerService.RateWasherAsync(userId, dto);
            return Ok(result);
        }

        [HttpGet("receipt/{orderId}")]
        public async Task<IActionResult> GetReceipt(int orderId)
        {
            var userId = GetUserId();
            var receipt = await _customerService.GetReceiptAsync(userId, orderId);
            return Ok(receipt);
        }

        [HttpGet("ratings")]
        public async Task<IActionResult> GetRatings()
        {
            var userId = GetUserId();
            var ratings = await _customerService.GetCustomerRatingsAsync(userId);
            return Ok(ratings);
        }
    }
}

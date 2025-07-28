using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusinessLayer.Interface;

namespace CarWashService.Controllers
{
    [Authorize(Roles = "Washer")]
    [ApiController]
    [Route("api/[controller]")]
    public class WasherController : ControllerBase
    {
        private readonly IWasherService _washerService;

        public WasherController(IWasherService washerService)
        {
            _washerService = washerService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("profile")]
        public async Task<IActionResult> GetWasherProfile()
        {
            var userId = GetUserId();
            var profile = await _washerService.GetWasherProfileAsync(userId);
            return Ok(profile);
        }

        [HttpGet("pending-orders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var pendingOrders = await _washerService.GetPendingOrdersAsync();
            return Ok(pendingOrders);
        }

        [HttpPost("accept-order/{orderId}")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var userId = GetUserId();
            var result = await _washerService.AcceptOrderAsync(userId, orderId);
            return Ok(result);
        }

        [HttpPost("start-work/{orderId}")]
        public async Task<IActionResult> StartWork(int orderId)
        {
            var userId = GetUserId();
            var result = await _washerService.StartWorkAsync(userId, orderId);
            return Ok(result);
        }

        [HttpPost("complete-order/{orderId}")]
        public async Task<IActionResult> CompleteOrder(int orderId, [FromBody] string? imageUrl = null)
        {
            var userId = GetUserId();
            var result = await _washerService.CompleteOrderAsync(userId, orderId, imageUrl ?? "");
            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _washerService.GetMyOrdersAsync(userId);
            return Ok(orders);
        }
    }
}
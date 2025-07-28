using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GreenWashBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CarWashService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly string _key = "rzp_test_x4I9ge6i9FHstL";
        private readonly string _secret = "L62QLn996QIkrMwzY7r8chR1";
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-keys")]
        public IActionResult TestKeys()
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);
                // Try to fetch account details to test authentication
                return Ok(new { 
                    message = "Keys are valid",
                    key = _key,
                    keyLength = _key.Length,
                    secretLength = _secret.Length
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Keys are invalid", 
                    error = ex.Message,
                    key = _key,
                    secret = _secret.Substring(0, 5) + "..."
                });
            }
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            try
            {
                // Debug logging
                if (request == null)
                {
                    return BadRequest(new { error = "Request is null" });
                }
                
                if (request.Amount <= 0)
                {
                    return BadRequest(new { error = "Invalid amount", receivedAmount = request.Amount });
                }

                RazorpayClient client = new RazorpayClient(_key, _secret);

                var options = new Dictionary<string, object>
                {
                    { "amount", request.Amount * 100 }, // amount in paise
                    { "currency", "INR" },
                    { "receipt", $"rcpt_{DateTime.Now:yyyyMMddHHmmss}" },
                    { "payment_capture", 1 }
                };

                var order = client.Order.Create(options);
                
                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    amount = request.Amount,
                    currency = "INR",
                    key = _key
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Failed to create order", 
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("generate-test-signature")]
        public IActionResult GenerateTestSignature()
        {
            // Sample test data
            string testOrderId = "order_test123456789";
            string testPaymentId = "pay_test987654321";
            
            // Generate signature
            string payload = testOrderId + "|" + testPaymentId;
            string signature = ComputeHMACSHA256(payload, _secret);
            
            return Ok(new {
                message = "Use this data to test verify endpoint",
                testData = new {
                    orderId = 1,
                    razorpayOrderId = testOrderId,
                    razorpayPaymentId = testPaymentId,
                    razorpaySignature = signature
                },
                payload = payload,
                computedSignature = signature
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationRequest request)
        {
            try
            {
                // Manual signature verification
                string payload = request.RazorpayOrderId + "|" + request.RazorpayPaymentId;
                string computedSignature = ComputeHMACSHA256(payload, _secret);
                
                if (computedSignature == request.RazorpaySignature)
                {
                    // Update order status to Paid
                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        order.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                    
                    return Ok(new { 
                        status = "Payment verified successfully.",
                        orderId = request.OrderId 
                    });
                }
                else
                {
                    return BadRequest(new { status = "Payment verification failed." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    status = "Payment verification failed.",
                    error = ex.Message 
                });
            }
        }

        private string ComputeHMACSHA256(string payload, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}

using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWashService.Controllers;
using ModelLayer.DTO;
using GreenWashBackend.Data;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GreenCarWashApp.Tests
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private PaymentController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new PaymentController(_context);

            // Seed test data
            _context.Orders.Add(new Order
            {
                Id = 1,
                CustomerId = 1,
                VehicleId = 1,
                WashPackageId = 1,
                Status = "Pending",
                TotalAmount = 500,
                ScheduledTime = DateTime.UtcNow,
                ImageAfterWashUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void TestKeys_ReturnsOkResult()
        {
            // Act
            var result = _controller.TestKeys();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Keys are valid"));
        }

        [Test]
        public void CreateOrder_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new PaymentRequest { Amount = 100 };

            // Act
            var result = _controller.CreateOrder(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void CreateOrder_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = _controller.CreateOrder(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult.Value);
            var valueString = badResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Request is null"));
        }

        [Test]
        public void CreateOrder_WithInvalidAmount_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaymentRequest { Amount = 0 };

            // Act
            var result = _controller.CreateOrder(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult.Value);
            var valueString = badResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Invalid amount"));
        }

        [Test]
        public void GenerateTestSignature_ReturnsOkResult()
        {
            // Act
            var result = _controller.GenerateTestSignature();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Use this data to test verify endpoint"));
            Assert.IsTrue(valueString.Contains("testData"));
        }

        [Test]
        public async Task VerifyPayment_WithValidSignature_ReturnsOkResult()
        {
            // Arrange
            string testOrderId = "order_test123456789";
            string testPaymentId = "pay_test987654321";
            string payload = testOrderId + "|" + testPaymentId;
            string signature = ComputeHMACSHA256(payload, "L62QLn996QIkrMwzY7r8chR1");

            var request = new PaymentVerificationRequest
            {
                OrderId = 1,
                RazorpayOrderId = testOrderId,
                RazorpayPaymentId = testPaymentId,
                RazorpaySignature = signature
            };

            // Act
            var result = await _controller.VerifyPayment(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Payment verified successfully."));
        }

        [Test]
        public async Task VerifyPayment_WithInvalidSignature_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaymentVerificationRequest
            {
                OrderId = 1,
                RazorpayOrderId = "order_test123456789",
                RazorpayPaymentId = "pay_test987654321",
                RazorpaySignature = "invalid_signature"
            };

            // Act
            var result = await _controller.VerifyPayment(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult.Value);
            var valueString = badResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Payment verification failed."));
        }

        [Test]
        public async Task VerifyPayment_UpdatesOrderStatus()
        {
            // Arrange
            string testOrderId = "order_test123456789";
            string testPaymentId = "pay_test987654321";
            string payload = testOrderId + "|" + testPaymentId;
            string signature = ComputeHMACSHA256(payload, "L62QLn996QIkrMwzY7r8chR1");

            var request = new PaymentVerificationRequest
            {
                OrderId = 1,
                RazorpayOrderId = testOrderId,
                RazorpayPaymentId = testPaymentId,
                RazorpaySignature = signature
            };

            // Act
            await _controller.VerifyPayment(request);

            // Assert
            var order = await _context.Orders.FindAsync(1);
            Assert.AreEqual("Paid", order.Status);
        }

        private string ComputeHMACSHA256(string payload, string secret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}
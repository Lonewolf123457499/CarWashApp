using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CarWashService.Controllers;
using BusinessLayer.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GreenCarWashApp.Tests
{
    [TestFixture]
    public class WasherControllerTests
    {
        private WasherController _controller;
        private Mock<IWasherService> _mockWasherService;

        [SetUp]
        public void Setup()
        {
            _mockWasherService = new Mock<IWasherService>();
            _controller = new WasherController(_mockWasherService.Object);

            // Mock washer user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Washer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task GetWasherProfile_ReturnsOkResult()
        {
            // Arrange
            var expectedProfile = new { Id = 1, Name = "John Washer", IsActive = true };
            _mockWasherService.Setup(x => x.GetWasherProfileAsync(1))
                             .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetWasherProfile();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedProfile, okResult.Value);
        }

        [Test]
        public async Task GetPendingOrders_ReturnsOkResult()
        {
            // Arrange
            var expectedOrders = new[] 
            { 
                new { Id = 1, Status = "Pending", CustomerName = "John Doe" },
                new { Id = 2, Status = "Pending", CustomerName = "Jane Smith" }
            };
            _mockWasherService.Setup(x => x.GetPendingOrdersAsync())
                             .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetPendingOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedOrders, okResult.Value);
        }

        [Test]
        public async Task AcceptOrder_WithValidOrderId_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var expectedResult = "Order accepted successfully";
            _mockWasherService.Setup(x => x.AcceptOrderAsync(1, orderId))
                             .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.AcceptOrder(orderId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task StartWork_WithValidOrderId_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var expectedResult = "Work started successfully";
            _mockWasherService.Setup(x => x.StartWorkAsync(1, orderId))
                             .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.StartWork(orderId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task CompleteOrder_WithValidOrderId_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var imageUrl = "https://example.com/image.jpg";
            var expectedResult = "Order completed successfully";
            _mockWasherService.Setup(x => x.CompleteOrderAsync(1, orderId, imageUrl))
                             .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CompleteOrder(orderId, imageUrl);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task CompleteOrder_WithoutImageUrl_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var expectedResult = "Order completed successfully";
            _mockWasherService.Setup(x => x.CompleteOrderAsync(1, orderId, ""))
                             .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CompleteOrder(orderId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetMyOrders_ReturnsOkResult()
        {
            // Arrange
            var expectedOrders = new[] 
            { 
                new { Id = 1, Status = "Completed", CustomerName = "John Doe" },
                new { Id = 2, Status = "InProgress", CustomerName = "Jane Smith" }
            };
            _mockWasherService.Setup(x => x.GetMyOrdersAsync(1))
                             .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetMyOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedOrders, okResult.Value);
        }

        [Test]
        public async Task AcceptOrder_CallsWasherService()
        {
            // Arrange
            var orderId = 1;

            // Act
            await _controller.AcceptOrder(orderId);

            // Assert
            _mockWasherService.Verify(x => x.AcceptOrderAsync(1, orderId), Times.Once);
        }

        [Test]
        public async Task StartWork_CallsWasherService()
        {
            // Arrange
            var orderId = 1;

            // Act
            await _controller.StartWork(orderId);

            // Assert
            _mockWasherService.Verify(x => x.StartWorkAsync(1, orderId), Times.Once);
        }

        [Test]
        public async Task CompleteOrder_CallsWasherService()
        {
            // Arrange
            var orderId = 1;
            var imageUrl = "test-image.jpg";

            // Act
            await _controller.CompleteOrder(orderId, imageUrl);

            // Assert
            _mockWasherService.Verify(x => x.CompleteOrderAsync(1, orderId, imageUrl), Times.Once);
        }

        [Test]
        public async Task GetWasherProfile_CallsWasherService()
        {
            // Act
            await _controller.GetWasherProfile();

            // Assert
            _mockWasherService.Verify(x => x.GetWasherProfileAsync(1), Times.Once);
        }

        [Test]
        public async Task GetMyOrders_CallsWasherService()
        {
            // Act
            await _controller.GetMyOrders();

            // Assert
            _mockWasherService.Verify(x => x.GetMyOrdersAsync(1), Times.Once);
        }
    }
}
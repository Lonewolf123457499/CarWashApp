using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CarWashService.Controllers;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using CarWashService.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GreenCarWashApp.Tests
{
    [TestFixture]
    public class CustomerControllerTests
    {
        private CustomerController _controller;
        private Mock<ICustomerService> _mockCustomerService;

        [SetUp]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _controller = new CustomerController(_mockCustomerService.Object);

            // Mock user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task GetProfile_ReturnsOkResult()
        {
            // Arrange
            var expectedProfile = new { Id = 1, Name = "John Doe" };
            _mockCustomerService.Setup(x => x.GetProfileAsync(1))
                               .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedProfile, okResult.Value);
        }

        [Test]
        public async Task UpdateProfile_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new UpdateCustomerDto
            {
                FullName = "John Updated",
                PhoneNumber = "9876543210"
            };
            var expectedResult = "Profile updated successfully";
            _mockCustomerService.Setup(x => x.UpdateProfileAsync(1, dto))
                               .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UpdateProfile(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task AddVehicle_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new VehicleDTO
            {
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123"
            };
            var expectedResult = "Vehicle added successfully";
            _mockCustomerService.Setup(x => x.AddVehicleAsync(1, dto))
                               .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.AddVehicle(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains(expectedResult));
        }

        [Test]
        public async Task GetVehicles_ReturnsOkResult()
        {
            // Arrange
            var expectedVehicles = new[] { new { Id = 1, Make = "Toyota" } };
            _mockCustomerService.Setup(x => x.GetVehiclesAsync(1))
                               .ReturnsAsync(expectedVehicles);

            // Act
            var result = await _controller.GetVehicles();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedVehicles, okResult.Value);
        }

        [Test]
        public async Task GetVehicle_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var vehicleId = 1;
            var expectedVehicle = new { Id = 1, Make = "Toyota" };
            _mockCustomerService.Setup(x => x.GetVehicleAsync(1, vehicleId))
                               .ReturnsAsync(expectedVehicle);

            // Act
            var result = await _controller.GetVehicle(vehicleId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedVehicle, okResult.Value);
        }

        [Test]
        public async Task DeleteVehicle_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var vehicleId = 1;
            var expectedResult = "Vehicle deleted successfully";
            _mockCustomerService.Setup(x => x.DeleteVehicleAsync(1, vehicleId))
                               .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.DeleteVehicle(vehicleId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task PlaceOrder_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new OrderDTO
            {
                VehicleId = 1,
                WashPackageId = 1,
                ScheduledTime = DateTime.Now.AddHours(2)
            };
            var expectedResult = "Order placed successfully";
            _mockCustomerService.Setup(x => x.PlaceOrderAsync(1, dto))
                               .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.PlaceOrder(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetOrders_ReturnsOkResult()
        {
            // Arrange
            var expectedOrders = new[] { new { Id = 1, Status = "Pending" } };
            _mockCustomerService.Setup(x => x.GetOrdersAsync(1))
                               .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedOrders, okResult.Value);
        }

        [Test]
        public async Task RateWasher_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new OrderRatingDTO
            {
                OrderId = 1,
                Rating = 5,
                Comment = "Excellent service"
            };
            var expectedResult = "Rating submitted successfully";
            _mockCustomerService.Setup(x => x.RateWasherAsync(1, dto))
                               .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.RateWasher(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetReceipt_WithValidOrderId_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var expectedReceipt = new { OrderId = 1, Amount = 100 };
            _mockCustomerService.Setup(x => x.GetReceiptAsync(1, orderId))
                               .ReturnsAsync(expectedReceipt);

            // Act
            var result = await _controller.GetReceipt(orderId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedReceipt, okResult.Value);
        }

        [Test]
        public async Task GetRatings_ReturnsOkResult()
        {
            // Arrange
            var expectedRatings = new[] { new { Id = 1, Rating = 5 } };
            _mockCustomerService.Setup(x => x.GetCustomerRatingsAsync(1))
                               .ReturnsAsync(expectedRatings);

            // Act
            var result = await _controller.GetRatings();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedRatings, okResult.Value);
        }
    }
}
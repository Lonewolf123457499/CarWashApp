using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using GreenCarWashApp.Controllers;
using ModelLayer.DTO;
using System.Threading.Tasks;

namespace GreenCarWashApp.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _controller;
        private Mock<IAuthService> _mockAuthService;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task RegisterCustomer_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password123",
                PhoneNumber = "1234567890",
                Role = "Customer"
            };

            _mockAuthService.Setup(x => x.RegisterCustomerAsync(It.IsAny<RegisterRequest>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterCustomer(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Customer registered successfully."));
        }

        [Test]
        public async Task RegisterWasher_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Jane Smith",
                Email = "jane@example.com",
                Password = "password123",
                PhoneNumber = "0987654321",
                Role = "Washer"
            };

            _mockAuthService.Setup(x => x.RegisterWasherAsync(It.IsAny<RegisterRequest>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterWasher(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var valueString = okResult.Value.ToString();
            Assert.IsTrue(valueString.Contains("Washer registered successfully."));
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var expectedResult = new { token = "jwt-token", role = "Customer" };
            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                           .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task RegisterCustomer_CallsAuthService()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "1234567890",
                Role = "Customer"
            };

            // Act
            await _controller.RegisterCustomer(request);

            // Assert
            _mockAuthService.Verify(x => x.RegisterCustomerAsync(request), Times.Once);
        }

        [Test]
        public async Task RegisterWasher_CallsAuthService()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test Washer",
                Email = "washer@example.com",
                Password = "password123",
                PhoneNumber = "1234567890",
                Role = "Washer"
            };

            // Act
            await _controller.RegisterWasher(request);

            // Assert
            _mockAuthService.Verify(x => x.RegisterWasherAsync(request), Times.Once);
        }

        [Test]
        public async Task Login_CallsAuthService()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "user@example.com",
                Password = "password123"
            };

            // Act
            await _controller.Login(request);

            // Assert
            _mockAuthService.Verify(x => x.LoginAsync(request), Times.Once);
        }
    }
}
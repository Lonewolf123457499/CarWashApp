using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Moq;
using GreenCarWashApp.Controllers;
using ModelLayer.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GreenCarWashApp.Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private AdminController _controller;
        private Mock<IAdminService> _mockAdminService;

        [SetUp]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);

            // Mock admin user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task AddWashPackage_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new AdminWashPackageDTO
            {
                Name = "Premium Wash",
                Description = "Complete car wash",
                Price = 500,
                Duration = 60
            };
            var expectedResult = "Wash package added successfully";
            _mockAdminService.Setup(x => x.AddWashPackageAsync(dto))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.AddWashPackage(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetWashPackages_ReturnsOkResult()
        {
            // Arrange
            var expectedPackages = new[] { new { Id = 1, Name = "Basic Wash" } };
            _mockAdminService.Setup(x => x.GetWashPackagesAsync())
                            .ReturnsAsync(expectedPackages);

            // Act
            var result = await _controller.GetWashPackages();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedPackages, okResult.Value);
        }

        [Test]
        public async Task UpdateWashPackage_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var packageId = 1;
            var dto = new AdminWashPackageDTO
            {
                Name = "Updated Package",
                Description = "Updated description",
                Price = 600,
                Duration = 90
            };
            var expectedResult = "Wash package updated successfully";
            _mockAdminService.Setup(x => x.UpdateWashPackageAsync(packageId, dto))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UpdateWashPackage(packageId, dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task DeleteWashPackage_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var packageId = 1;
            var expectedResult = "Wash package deleted successfully";
            _mockAdminService.Setup(x => x.DeleteWashPackageAsync(packageId))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.DeleteWashPackage(packageId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task AddAddon_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new AdminAddonDTO
            {
                Name = "Wax Polish",
                Description = "Premium wax polish",
                Price = 200
            };
            var expectedResult = "Addon added successfully";
            _mockAdminService.Setup(x => x.AddAddonAsync(dto))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.AddAddon(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetAddons_ReturnsOkResult()
        {
            // Arrange
            var expectedAddons = new[] { new { Id = 1, Name = "Air Freshener" } };
            _mockAdminService.Setup(x => x.GetAddonsAsync())
                            .ReturnsAsync(expectedAddons);

            // Act
            var result = await _controller.GetAddons();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedAddons, okResult.Value);
        }

        [Test]
        public async Task GetWashers_ReturnsOkResult()
        {
            // Arrange
            var expectedWashers = new[] { new { Id = 1, Name = "John Washer" } };
            _mockAdminService.Setup(x => x.GetWashersAsync())
                            .ReturnsAsync(expectedWashers);

            // Act
            var result = await _controller.GetWashers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedWashers, okResult.Value);
        }

        [Test]
        public async Task UpdateWasherStatus_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var washerId = 1;
            var dto = new WasherStatusDTO { IsActive = true };
            var expectedResult = "Washer status updated successfully";
            _mockAdminService.Setup(x => x.UpdateWasherStatusAsync(washerId, dto))
                            .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UpdateWasherStatus(washerId, dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResult, okResult.Value);
        }

        [Test]
        public async Task GetCustomers_ReturnsOkResult()
        {
            // Arrange
            var expectedCustomers = new[] { new { Id = 1, Name = "Jane Customer" } };
            _mockAdminService.Setup(x => x.GetCustomersAsync())
                            .ReturnsAsync(expectedCustomers);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedCustomers, okResult.Value);
        }

        [Test]
        public async Task GetAllOrders_ReturnsOkResult()
        {
            // Arrange
            var expectedOrders = new[] { new { Id = 1, Status = "Completed" } };
            _mockAdminService.Setup(x => x.GetAllOrdersAsync())
                            .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedOrders, okResult.Value);
        }

        [Test]
        public async Task GetOrderStats_ReturnsOkResult()
        {
            // Arrange
            var expectedStats = new { TotalOrders = 100, CompletedOrders = 80 };
            _mockAdminService.Setup(x => x.GetOrderStatsAsync())
                            .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GetOrderStats();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedStats, okResult.Value);
        }

        [Test]
        public async Task GetDashboardStats_ReturnsOkResult()
        {
            // Arrange
            var expectedStats = new { TotalCustomers = 50, TotalWashers = 10, TotalRevenue = 50000 };
            _mockAdminService.Setup(x => x.GetDashboardStatsAsync())
                            .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GetDashboardStats();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedStats, okResult.Value);
        }
    }
}
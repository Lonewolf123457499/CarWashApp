using ModelLayer.DTO;
using BusinessLayer.Interface;
using RepositoryLayer.Interface;
using CarWashService.DTOs;

namespace BusinessLayer.Service
{
    public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // Profile Management
    public async Task<object> GetProfileAsync(int userId)
    {
        var profile = await _customerRepository.GetCustomerProfileAsync(userId);
        if (profile == null) throw new KeyNotFoundException("Customer not found");
        return profile;
    }

    public async Task<string> UpdateProfileAsync(int userId, CarWashService.DTOs.UpdateCustomerDto dto)
    {
        var updated = await _customerRepository.UpdateCustomerProfileAsync(userId, dto);
        if (!updated) throw new KeyNotFoundException("Customer not found");
        return "Profile updated";
    }

    // Vehicle Management
    public async Task<string> AddVehicleAsync(int userId, VehicleDTO dto)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found. Please register as a customer before adding vehicles.");

        var vehicle = new Vehicle
        {
            CustomerId = customer.Id,
            Make = dto.Make,
            Model = dto.Model,
            LicensePlate = dto.NumberPlate,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddVehicleAsync(vehicle);
        return "Vehicle added successfully.";
    }

    public async Task<IEnumerable<object>> GetVehiclesAsync(int userId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");
        
        return await _customerRepository.GetVehiclesAsync(customer.Id);
    }

    public async Task<object> GetVehicleAsync(int userId, int vehicleId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");
        
        var vehicle = await _customerRepository.GetVehicleAsync(vehicleId, customer.Id);
        if (vehicle == null) throw new KeyNotFoundException("Vehicle not found");
        return vehicle;
    }

    public async Task<string> DeleteVehicleAsync(int userId, int vehicleId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        var hasActiveOrders = await _customerRepository.VehicleHasActiveOrdersAsync(vehicleId);
        if (hasActiveOrders) throw new InvalidOperationException("Cannot delete vehicle. It is assigned to a washer or service is ongoing.");

        var deleted = await _customerRepository.DeleteVehicleAsync(vehicleId, customer.Id);
        if (!deleted) throw new KeyNotFoundException("Vehicle not found");
        
        return "Vehicle deleted successfully";
    }

    // Order Management
    public async Task<object> PlaceOrderAsync(int userId, OrderDTO dto)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        // Verify vehicle belongs to customer
        var vehicleBelongs = await _customerRepository.VehicleBelongsToCustomerAsync(dto.VehicleId, customer.Id);
        if (!vehicleBelongs) throw new KeyNotFoundException("Vehicle not found or does not belong to customer");

        // Verify wash package exists
        var washPackage = await _customerRepository.GetWashPackageAsync(dto.WashPackageId);
        if (washPackage == null) throw new KeyNotFoundException("Wash package not found");

        // Calculate total amount
        decimal totalAmount = washPackage.Price;
        
        if (dto.AddonIds?.Any() == true)
        {
            var addons = await _customerRepository.GetAddonsAsync(dto.AddonIds);
            if (addons.Count != dto.AddonIds.Count)
                throw new ArgumentException("One or more addon IDs are invalid");
            totalAmount += addons.Sum(a => a.Price);
        }

        var order = new Order
        {
            CustomerId = customer.Id,
            VehicleId = dto.VehicleId,
            WashPackageId = dto.WashPackageId,
            WasherId = null,
            ScheduledTime = dto.OrderDate,
            Status = "Pending",
            TotalAmount = totalAmount,
            ImageAfterWashUrl = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var placedOrder = await _customerRepository.PlaceOrderAsync(order);

        // Add order addons if any
        if (dto.AddonIds?.Any() == true)
        {
            var orderAddons = dto.AddonIds.Select(addonId => new OrderAddon
            {
                OrderId = placedOrder.Id,
                AddonId = addonId
            }).ToList();

            await _customerRepository.AddOrderAddonsAsync(orderAddons);
        }

        return new { Message = "Order placed successfully", OrderId = placedOrder.Id, TotalAmount = totalAmount };
    }

    public async Task<IEnumerable<object>> GetOrdersAsync(int userId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        return await _customerRepository.GetOrdersAsync(customer.Id);
    }

    // Rating Management
    public async Task<string> RateWasherAsync(int userId, OrderRatingDTO dto)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        var orderExists = await _customerRepository.OrderExistsAndCompletedAsync(dto.OrderId, customer.Id);
        if (!orderExists) throw new InvalidOperationException("Order not found, not completed/paid, or no washer assigned");

        var ratingExists = await _customerRepository.RatingExistsAsync(dto.OrderId);
        if (ratingExists) throw new InvalidOperationException("Order already rated");

        // Use the rating value directly since it's now an int
        int starCount = Math.Max(0, Math.Min(5, dto.Rating)); // Clamp between 0-5

        var rating = new Rating
        {
            OrderId = dto.OrderId,
            Stars = starCount,
            Comment = dto.Comment ?? dto.Review ?? "",
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddRatingAsync(rating);
        return "Rating submitted successfully";
    }

    // Receipt Management
    public async Task<object> GetReceiptAsync(int userId, int orderId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        var receipt = await _customerRepository.GetReceiptAsync(orderId, customer.Id);
        if (receipt == null) throw new KeyNotFoundException("Receipt not found");
        return receipt;
    }

    // Rating Management
    public async Task<IEnumerable<object>> GetCustomerRatingsAsync(int userId)
    {
        var customer = await _customerRepository.GetCustomerByUserIdAsync(userId);
        if (customer == null) throw new KeyNotFoundException("Customer not found");

        return await _customerRepository.GetCustomerRatingsAsync(customer.Id);
    }
    }
}
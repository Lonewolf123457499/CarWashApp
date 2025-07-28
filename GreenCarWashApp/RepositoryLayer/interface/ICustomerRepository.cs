using ModelLayer.DTO;
using CarWashService.DTOs;

namespace RepositoryLayer.Interface
{
    public interface ICustomerRepository
{
    // Profile Management
    Task<Customer> GetCustomerByUserIdAsync(int userId);
    Task<object> GetCustomerProfileAsync(int userId);
    Task<bool> UpdateCustomerProfileAsync(int userId, CarWashService.DTOs.UpdateCustomerDto dto);

    // Vehicle Management
    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    Task<IEnumerable<object>> GetVehiclesAsync(int customerId);
    Task<object> GetVehicleAsync(int vehicleId, int customerId);
    Task<bool> DeleteVehicleAsync(int vehicleId, int customerId);
    Task<bool> VehicleHasActiveOrdersAsync(int vehicleId);

    // Order Management
    Task<Order> PlaceOrderAsync(Order order);
    Task AddOrderAddonsAsync(List<OrderAddon> orderAddons);
    Task<IEnumerable<object>> GetOrdersAsync(int customerId);
    Task<bool> VehicleBelongsToCustomerAsync(int vehicleId, int customerId);
    Task<WashPackage> GetWashPackageAsync(int washPackageId);
    Task<List<Addon>> GetAddonsAsync(List<int> addonIds);

    // Rating Management
    Task<bool> AddRatingAsync(Rating rating);
    Task<bool> OrderExistsAndCompletedAsync(int orderId, int customerId);
    Task<bool> RatingExistsAsync(int orderId);

    // Receipt Management
    Task<object> GetReceiptAsync(int orderId, int customerId);
    
    // Rating Management
    Task<IEnumerable<object>> GetCustomerRatingsAsync(int customerId);
}
}
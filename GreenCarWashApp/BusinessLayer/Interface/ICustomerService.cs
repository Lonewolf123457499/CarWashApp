using ModelLayer.DTO;
using CarWashService.DTOs;

namespace BusinessLayer.Interface
{
    public interface ICustomerService
{
    // Profile Management
    Task<object> GetProfileAsync(int userId);
    Task<string> UpdateProfileAsync(int userId, CarWashService.DTOs.UpdateCustomerDto dto);

    // Vehicle Management
    Task<string> AddVehicleAsync(int userId, VehicleDTO dto);
    Task<IEnumerable<object>> GetVehiclesAsync(int userId);
    Task<object> GetVehicleAsync(int userId, int vehicleId);
    Task<string> DeleteVehicleAsync(int userId, int vehicleId);

    // Order Management
    Task<object> PlaceOrderAsync(int userId, OrderDTO dto);
    Task<IEnumerable<object>> GetOrdersAsync(int userId);

    // Rating Management
    Task<string> RateWasherAsync(int userId, OrderRatingDTO dto);

    // Receipt Management
    Task<object> GetReceiptAsync(int userId, int orderId);
    
    // Rating Management
    Task<IEnumerable<object>> GetCustomerRatingsAsync(int userId);
    }
}
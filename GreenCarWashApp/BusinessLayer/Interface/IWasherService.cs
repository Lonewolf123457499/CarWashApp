using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IWasherService
    {
        // Profile Management
        Task<object> GetWasherProfileAsync(int userId);
        
        // Order Management
        Task<IEnumerable<object>> GetPendingOrdersAsync();
        Task<string> AcceptOrderAsync(int userId, int orderId);
        Task<string> StartWorkAsync(int userId, int orderId);
        Task<string> CompleteOrderAsync(int userId, int orderId, string imageUrl);
        Task<IEnumerable<object>> GetMyOrdersAsync(int userId);
    }
}
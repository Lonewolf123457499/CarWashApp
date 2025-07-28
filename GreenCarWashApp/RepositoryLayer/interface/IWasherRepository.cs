using ModelLayer.DTO;

namespace RepositoryLayer.Interface
{
    public interface IWasherRepository
    {
        // Washer Management
        Task<Washer> GetWasherByUserIdAsync(int userId);
        Task<object> GetWasherProfileAsync(int userId);

        // Order Management
        Task<IEnumerable<object>> GetPendingOrdersAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderByIdAndWasherAsync(int orderId, int washerId);
        Task<bool> AcceptOrderAsync(int orderId, int washerId);
        Task<bool> StartWorkAsync(int orderId);
        Task<bool> CompleteOrderAsync(int orderId, string imageUrl);
        Task<IEnumerable<object>> GetWasherOrdersAsync(int washerId);

        // Receipt Management
        Task<Receipt> AddReceiptAsync(Receipt receipt);
    }
}
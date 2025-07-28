using ModelLayer.DTO;

public interface IAdminService
{
    // Wash Package Management
    Task<object> AddWashPackageAsync(AdminWashPackageDTO dto);
    Task<IEnumerable<object>> GetWashPackagesAsync();
    Task<string> UpdateWashPackageAsync(int id, AdminWashPackageDTO dto);
    Task<string> DeleteWashPackageAsync(int id);

    // Addon Management
    Task<object> AddAddonAsync(AdminAddonDTO dto);
    Task<IEnumerable<object>> GetAddonsAsync();
    Task<string> UpdateAddonAsync(int id, AdminAddonDTO dto);
    Task<string> DeleteAddonAsync(int id);

    // Washer Management
    Task<IEnumerable<object>> GetWashersAsync();
    Task<string> UpdateWasherStatusAsync(int id, WasherStatusDTO dto);

    // Customer Management
    Task<IEnumerable<object>> GetCustomersAsync();

    // Order Management
    Task<IEnumerable<object>> GetAllOrdersAsync();
    Task<object> GetOrderStatsAsync();
    Task<object> GetDashboardStatsAsync();
}
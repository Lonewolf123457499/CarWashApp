using ModelLayer.DTO;

public interface IAdminRepository
{
    // Wash Package Management
    Task<WashPackage> AddWashPackageAsync(WashPackage washPackage);
    Task<IEnumerable<WashPackage>> GetWashPackagesAsync();
    Task<WashPackage> UpdateWashPackageAsync(WashPackage washPackage);
    Task<bool> DeleteWashPackageAsync(int id);
    Task<bool> WashPackageHasOrdersAsync(int id);

    // Addon Management
    Task<Addon> AddAddonAsync(Addon addon);
    Task<IEnumerable<Addon>> GetAddonsAsync();
    Task<Addon> UpdateAddonAsync(Addon addon);
    Task<bool> DeleteAddonAsync(int id);
    Task<bool> AddonHasOrdersAsync(int id);

    // Washer Management
    Task<IEnumerable<object>> GetWashersAsync();
    Task<bool> UpdateWasherStatusAsync(int id, bool isActive);

    // Customer Management
    Task<IEnumerable<object>> GetCustomersAsync();

    // Order Management
    Task<IEnumerable<object>> GetAllOrdersAsync();
    Task<object> GetOrderStatsAsync();
    Task<object> GetDashboardStatsAsync();
}
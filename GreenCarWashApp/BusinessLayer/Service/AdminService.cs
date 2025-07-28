using ModelLayer.DTO;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    // Wash Package Management
    public async Task<object> AddWashPackageAsync(AdminWashPackageDTO dto)
    {
        if (dto == null) throw new ArgumentException("Wash package data is required");
        if (string.IsNullOrEmpty(dto.Name)) throw new ArgumentException("Name is required");
        if (dto.Price <= 0) throw new ArgumentException("Price must be greater than 0");

        var washPackage = new WashPackage
        {
            Name = dto.Name,
            Description = dto.Description ?? "",
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _adminRepository.AddWashPackageAsync(washPackage);
        return new { Message = "Wash package added successfully", Id = result.Id };
    }

    public async Task<IEnumerable<object>> GetWashPackagesAsync()
    {
        var packages = await _adminRepository.GetWashPackagesAsync();
        return packages.Select(wp => new { wp.Id, wp.Name, wp.Description, wp.Price, wp.CreatedAt });
    }

    public async Task<string> UpdateWashPackageAsync(int id, AdminWashPackageDTO dto)
    {
        var package = await _adminRepository.GetWashPackagesAsync();
        var existingPackage = package.FirstOrDefault(p => p.Id == id);
        if (existingPackage == null) throw new KeyNotFoundException("Wash package not found");

        existingPackage.Name = dto.Name ?? existingPackage.Name;
        existingPackage.Description = dto.Description ?? existingPackage.Description;
        existingPackage.Price = dto.Price > 0 ? dto.Price : existingPackage.Price;
        existingPackage.UpdatedAt = DateTime.UtcNow;

        await _adminRepository.UpdateWashPackageAsync(existingPackage);
        return "Wash package updated successfully";
    }

    public async Task<string> DeleteWashPackageAsync(int id)
    {
        var hasOrders = await _adminRepository.WashPackageHasOrdersAsync(id);
        if (hasOrders) throw new InvalidOperationException("Cannot delete wash package with existing orders");

        var deleted = await _adminRepository.DeleteWashPackageAsync(id);
        if (!deleted) throw new KeyNotFoundException("Wash package not found");

        return "Wash package deleted successfully";
    }

    // Addon Management
    public async Task<object> AddAddonAsync(AdminAddonDTO dto)
    {
        if (dto == null) throw new ArgumentException("Addon data is required");
        if (string.IsNullOrEmpty(dto.Name)) throw new ArgumentException("Name is required");
        if (dto.Price <= 0) throw new ArgumentException("Price must be greater than 0");

        var addon = new Addon
        {
            Name = dto.Name,
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _adminRepository.AddAddonAsync(addon);
        return new { Message = "Addon added successfully", Id = result.Id };
    }

    public async Task<IEnumerable<object>> GetAddonsAsync()
    {
        var addons = await _adminRepository.GetAddonsAsync();
        return addons.Select(a => new { a.Id, a.Name, a.Price, a.CreatedAt });
    }

    public async Task<string> UpdateAddonAsync(int id, AdminAddonDTO dto)
    {
        var addons = await _adminRepository.GetAddonsAsync();
        var existingAddon = addons.FirstOrDefault(a => a.Id == id);
        if (existingAddon == null) throw new KeyNotFoundException("Addon not found");

        existingAddon.Name = dto.Name ?? existingAddon.Name;
        existingAddon.Price = dto.Price > 0 ? dto.Price : existingAddon.Price;
        existingAddon.UpdatedAt = DateTime.UtcNow;

        await _adminRepository.UpdateAddonAsync(existingAddon);
        return "Addon updated successfully";
    }

    public async Task<string> DeleteAddonAsync(int id)
    {
        var hasOrders = await _adminRepository.AddonHasOrdersAsync(id);
        if (hasOrders) throw new InvalidOperationException("Cannot delete addon with existing orders");

        var deleted = await _adminRepository.DeleteAddonAsync(id);
        if (!deleted) throw new KeyNotFoundException("Addon not found");

        return "Addon deleted successfully";
    }

    // Washer Management
    public async Task<IEnumerable<object>> GetWashersAsync()
    {
        return await _adminRepository.GetWashersAsync();
    }

    public async Task<string> UpdateWasherStatusAsync(int id, WasherStatusDTO dto)
    {
        var updated = await _adminRepository.UpdateWasherStatusAsync(id, dto.IsActive);
        if (!updated) throw new KeyNotFoundException("Washer not found");

        return $"Washer {(dto.IsActive ? "activated" : "deactivated")} successfully";
    }

    // Customer Management
    public async Task<IEnumerable<object>> GetCustomersAsync()
    {
        return await _adminRepository.GetCustomersAsync();
    }

    // Order Management
    public async Task<IEnumerable<object>> GetAllOrdersAsync()
    {
        return await _adminRepository.GetAllOrdersAsync();
    }

    public async Task<object> GetOrderStatsAsync()
    {
        return await _adminRepository.GetOrderStatsAsync();
    }

    public async Task<object> GetDashboardStatsAsync()
    {
        return await _adminRepository.GetDashboardStatsAsync();
    }
}
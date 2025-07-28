using GreenWashBackend.Data;
using Microsoft.EntityFrameworkCore;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Wash Package Management
    public async Task<WashPackage> AddWashPackageAsync(WashPackage washPackage)
    {
        _context.WashPackages.Add(washPackage);
        await _context.SaveChangesAsync();
        return washPackage;
    }

    public async Task<IEnumerable<WashPackage>> GetWashPackagesAsync()
    {
        return await _context.WashPackages.ToListAsync();
    }

    public async Task<WashPackage> UpdateWashPackageAsync(WashPackage washPackage)
    {
        _context.WashPackages.Update(washPackage);
        await _context.SaveChangesAsync();
        return washPackage;
    }

    public async Task<bool> DeleteWashPackageAsync(int id)
    {
        var package = await _context.WashPackages.FindAsync(id);
        if (package == null) return false;

        _context.WashPackages.Remove(package);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> WashPackageHasOrdersAsync(int id)
    {
        return await _context.Orders.AnyAsync(o => o.WashPackageId == id);
    }

    // Addon Management
    public async Task<Addon> AddAddonAsync(Addon addon)
    {
        _context.Addons.Add(addon);
        await _context.SaveChangesAsync();
        return addon;
    }

    public async Task<IEnumerable<Addon>> GetAddonsAsync()
    {
        return await _context.Addons.ToListAsync();
    }

    public async Task<Addon> UpdateAddonAsync(Addon addon)
    {
        _context.Addons.Update(addon);
        await _context.SaveChangesAsync();
        return addon;
    }

    public async Task<bool> DeleteAddonAsync(int id)
    {
        var addon = await _context.Addons.FindAsync(id);
        if (addon == null) return false;

        _context.Addons.Remove(addon);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddonHasOrdersAsync(int id)
    {
        return await _context.OrderAddons.AnyAsync(oa => oa.AddonId == id);
    }

    // Washer Management
    public async Task<IEnumerable<object>> GetWashersAsync()
    {
        return await _context.Washers
            .Select(w => new {
                w.Id,
                w.FullName,
                w.Email,
                w.Phone,
                w.IsActive,
                w.CreatedAt,
                OrderCount = w.Orders.Count(),
                CompletedOrders = w.Orders.Count(o => o.Status == "Completed")
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateWasherStatusAsync(int id, bool isActive)
    {
        var washer = await _context.Washers.FindAsync(id);
        if (washer == null) return false;

        washer.IsActive = isActive;
        washer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    // Customer Management
    public async Task<IEnumerable<object>> GetCustomersAsync()
    {
        return await _context.Customers
            .Select(c => new {
                c.Id,
                c.FullName,
                c.Email,
                c.Phone,
                c.CreatedAt,
                VehicleCount = c.Vehicles.Count(),
                OrderCount = c.Orders.Count(),
                TotalSpent = c.Orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount)
            })
            .ToListAsync();
    }

    // Order Management
    public async Task<IEnumerable<object>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Washer)
            .Include(o => o.Vehicle)
            .Include(o => o.WashPackage)
            .Include(o => o.OrderAddons)
                .ThenInclude(oa => oa.Addon)
            .Select(o => new {
                o.Id,
                o.ScheduledTime,
                o.CompletedTime,
                o.Status,
                o.TotalAmount,
                Customer = new { o.Customer.FullName, o.Customer.Email },
                Washer = o.Washer != null ? new { o.Washer.FullName, o.Washer.Email } : null,
                Vehicle = new { o.Vehicle.Make, o.Vehicle.Model, o.Vehicle.LicensePlate },
                WashPackage = new { o.WashPackage.Name, o.WashPackage.Price },
                Addons = o.OrderAddons.Select(oa => new { oa.Addon.Name, oa.Addon.Price }).ToList(),
                o.CreatedAt
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<object> GetOrderStatsAsync()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
        var inProgressOrders = await _context.Orders.CountAsync(o => o.Status == "InProgress");
        var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
        var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "Cancelled");
        var totalRevenue = await _context.Orders
            .Where(o => o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);

        return new {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            InProgressOrders = inProgressOrders,
            CompletedOrders = completedOrders,
            CancelledOrders = cancelledOrders,
            TotalRevenue = totalRevenue
        };
    }

    public async Task<object> GetDashboardStatsAsync()
    {
        var totalCustomers = await _context.Customers.CountAsync();
        var totalWashers = await _context.Washers.CountAsync();
        var activeWashers = await _context.Washers.CountAsync(w => w.IsActive);
        var totalWashPackages = await _context.WashPackages.CountAsync();
        var totalAddons = await _context.Addons.CountAsync();
        
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
        var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
        var totalRevenue = await _context.Orders
            .Where(o => o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);

        return new {
            TotalCustomers = totalCustomers,
            TotalWashers = totalWashers,
            ActiveWashers = activeWashers,
            TotalWashPackages = totalWashPackages,
            TotalAddons = totalAddons,
            OrderStats = new {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                TotalRevenue = totalRevenue
            }
        };
    }
}
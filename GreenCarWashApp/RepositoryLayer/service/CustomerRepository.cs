using GreenWashBackend.Data;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Interface;
using CarWashService.DTOs;

namespace RepositoryLayer.Service
{
    public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Profile Management
    public async Task<Customer> GetCustomerByUserIdAsync(int userId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<object> GetCustomerProfileAsync(int userId)
    {
        return await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => new { c.Id, c.FullName, c.Email, c.Phone, c.CreatedAt, c.UpdatedAt })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateCustomerProfileAsync(int userId, CarWashService.DTOs.UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer == null) return false;

        customer.FullName = dto.FullName;
        customer.Phone = dto.PhoneNumber;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Vehicle Management
    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    public async Task<IEnumerable<object>> GetVehiclesAsync(int customerId)
    {
        return await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .Select(v => new { v.Id, v.Make, v.Model, v.LicensePlate, v.ImageUrl, v.CreatedAt, v.UpdatedAt })
            .ToListAsync();
    }

    public async Task<object> GetVehicleAsync(int vehicleId, int customerId)
    {
        return await _context.Vehicles
            .Where(v => v.Id == vehicleId && v.CustomerId == customerId)
            .Select(v => new { v.Id, v.Make, v.Model, v.LicensePlate, v.ImageUrl, v.CreatedAt, v.UpdatedAt })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId, int customerId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);
        if (vehicle == null) return false;

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VehicleHasActiveOrdersAsync(int vehicleId)
    {
        return await _context.Orders
            .AnyAsync(o => o.VehicleId == vehicleId && 
                (o.Status == "Assigned" || o.Status == "InProgress"));
    }

    // Order Management
    public async Task<Order> PlaceOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task AddOrderAddonsAsync(List<OrderAddon> orderAddons)
    {
        _context.OrderAddons.AddRange(orderAddons);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<object>> GetOrdersAsync(int customerId)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Select(o => new {
                o.Id,
                o.ScheduledTime,
                o.Status,
                o.TotalAmount,
                Vehicle = new { o.Vehicle.Make, o.Vehicle.Model, o.Vehicle.LicensePlate },
                WashPackage = new { o.WashPackage.Name, o.WashPackage.Description }
            })
            .OrderByDescending(o => o.ScheduledTime)
            .ToListAsync();
    }

    public async Task<bool> VehicleBelongsToCustomerAsync(int vehicleId, int customerId)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.Id == vehicleId && v.CustomerId == customerId);
    }

    public async Task<WashPackage> GetWashPackageAsync(int washPackageId)
    {
        return await _context.WashPackages.FindAsync(washPackageId);
    }

    public async Task<List<Addon>> GetAddonsAsync(List<int> addonIds)
    {
        return await _context.Addons
            .Where(a => addonIds.Contains(a.Id))
            .ToListAsync();
    }

    // Rating Management
    public async Task<bool> AddRatingAsync(Rating rating)
    {
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> OrderExistsAndCompletedAsync(int orderId, int customerId)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == orderId && o.CustomerId == customerId && 
                (o.Status == "Completed" || o.Status == "Paid") && o.WasherId != null);
    }

    public async Task<bool> RatingExistsAsync(int orderId)
    {
        return await _context.Ratings.AnyAsync(r => r.OrderId == orderId);
    }

    // Receipt Management
    public async Task<object> GetReceiptAsync(int orderId, int customerId)
    {
        return await _context.Receipts
            .Include(r => r.Order)
            .ThenInclude(o => o.Vehicle)
            .Include(r => r.Order)
            .ThenInclude(o => o.WashPackage)
            .Where(r => r.Order.CustomerId == customerId && r.OrderId == orderId)
            .Select(r => new {
                r.ReceiptNumber,
                r.Details,
                r.Total,
                r.CreatedAt,
                OrderId = r.Order.Id,
                OrderStatus = r.Order.Status
            })
            .FirstOrDefaultAsync();
    }

    // Rating Management
    public async Task<IEnumerable<object>> GetCustomerRatingsAsync(int customerId)
    {
        return await _context.Ratings
            .Include(r => r.Order)
            .ThenInclude(o => o.WashPackage)
            .Include(r => r.Order)
            .ThenInclude(o => o.Vehicle)
            .Where(r => r.Order.CustomerId == customerId)
            .Select(r => new {
                r.Id,
                r.OrderId,
                r.Stars,
                r.Comment,
                r.CreatedAt,
                Order = new {
                    r.Order.Id,
                    r.Order.Status,
                    r.Order.TotalAmount,
                    r.Order.ScheduledTime,
                    WashPackage = r.Order.WashPackage.Name,
                    Vehicle = $"{r.Order.Vehicle.Make} {r.Order.Vehicle.Model}"
                }
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    }
}
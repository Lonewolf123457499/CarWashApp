using GreenWashBackend.Data;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class WasherRepository : IWasherRepository
    {
        private readonly ApplicationDbContext _context;

        public WasherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Washer Management
        public async Task<Washer> GetWasherByUserIdAsync(int userId)
        {
            return await _context.Washers.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<object> GetWasherProfileAsync(int userId)
        {
            return await _context.Washers
                .Include(w => w.User)
                .Where(w => w.UserId == userId)
                .Select(w => new {
                    w.Id,
                    w.User.FullName,
                    w.User.Email,
                    w.Phone,
                    w.IsActive,
                    w.CreatedAt,
                    w.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        // Order Management
        public async Task<IEnumerable<object>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Pending")
                .Select(o => new {
                    o.Id,
                    o.ScheduledTime,
                    o.TotalAmount,
                    Customer = new { o.Customer.FullName, o.Customer.Phone },
                    Vehicle = new { o.Vehicle.Make, o.Vehicle.Model, o.Vehicle.LicensePlate },
                    WashPackage = new { o.WashPackage.Name, o.WashPackage.Description },
                    CreatedAt = o.CreatedAt
                })
                .OrderBy(o => o.ScheduledTime)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .Include(o => o.WashPackage)
                .Include(o => o.OrderAddons)
                .ThenInclude(oa => oa.Addon)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order> GetOrderByIdAndWasherAsync(int orderId, int washerId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.WasherId == washerId);
        }

        public async Task<bool> AcceptOrderAsync(int orderId, int washerId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Pending") return false;

            order.WasherId = washerId;
            order.Status = "Assigned";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartWorkAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Assigned") return false;

            order.Status = "InProgress";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteOrderAsync(int orderId, string imageUrl)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "InProgress") return false;

            order.Status = "Completed";
            order.CompletedTime = DateTime.UtcNow;
            order.ImageAfterWashUrl = imageUrl ?? "";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetWasherOrdersAsync(int washerId)
        {
            return await _context.Orders
                .Where(o => o.WasherId == washerId)
                .Select(o => new {
                    o.Id,
                    o.ScheduledTime,
                    o.Status,
                    o.TotalAmount,
                    Customer = new { o.Customer.FullName, o.Customer.Phone },
                    Vehicle = new { o.Vehicle.Make, o.Vehicle.Model, o.Vehicle.LicensePlate },
                    WashPackage = new { o.WashPackage.Name }
                })
                .OrderByDescending(o => o.ScheduledTime)
                .ToListAsync();
        }

        // Receipt Management
        public async Task<Receipt> AddReceiptAsync(Receipt receipt)
        {
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }
    }
}
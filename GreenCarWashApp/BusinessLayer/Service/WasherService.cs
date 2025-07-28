using ModelLayer.DTO;
using BusinessLayer.Interface;
using RepositoryLayer.Interface;
using BusinessLayer.Helper;

namespace BusinessLayer.Service
{
    public class WasherService : IWasherService
    {
        private readonly IWasherRepository _washerRepository;
        private readonly IEmailService _emailService;

        public WasherService(IWasherRepository washerRepository, IEmailService emailService)
        {
            _washerRepository = washerRepository;
            _emailService = emailService;
        }

        public async Task<object> GetWasherProfileAsync(int userId)
        {
            var profile = await _washerRepository.GetWasherProfileAsync(userId);
            if (profile == null) throw new KeyNotFoundException("Washer not found");
            return profile;
        }
        
        public async Task<IEnumerable<object>> GetPendingOrdersAsync()
        {
            return await _washerRepository.GetPendingOrdersAsync();
        }

        public async Task<string> AcceptOrderAsync(int userId, int orderId)
        {
            var washer = await _washerRepository.GetWasherByUserIdAsync(userId);
            if (washer == null) throw new KeyNotFoundException("Washer not found");

            var order = await _washerRepository.GetOrderByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            if (order.Status != "Pending") throw new InvalidOperationException("Order is not available");

            var accepted = await _washerRepository.AcceptOrderAsync(orderId, washer.Id);
            if (!accepted) throw new InvalidOperationException("Failed to accept order");

            // Generate receipt
            var receipt = GenerateReceipt(order);
            await _washerRepository.AddReceiptAsync(receipt);

            // Send email notification
            await SendOrderAcceptedNotification(order, washer);

            return "Order accepted successfully";
        }

        public async Task<string> StartWorkAsync(int userId, int orderId)
        {
            var washer = await _washerRepository.GetWasherByUserIdAsync(userId);
            if (washer == null) throw new KeyNotFoundException("Washer not found");

            var order = await _washerRepository.GetOrderByIdAndWasherAsync(orderId, washer.Id);
            if (order == null) throw new KeyNotFoundException("Order not found or not assigned to you");
            if (order.Status != "Assigned") throw new InvalidOperationException("Order is not ready to start");

            var started = await _washerRepository.StartWorkAsync(orderId);
            if (!started) throw new InvalidOperationException("Failed to start work");

            // Send notification
            await SendWorkStartedNotification(order, washer);

            return "Work started successfully";
        }

        public async Task<string> CompleteOrderAsync(int userId, int orderId, string imageUrl)
        {
            var washer = await _washerRepository.GetWasherByUserIdAsync(userId);
            if (washer == null) throw new KeyNotFoundException("Washer not found");

            var order = await _washerRepository.GetOrderByIdAndWasherAsync(orderId, washer.Id);
            if (order == null) throw new KeyNotFoundException("Order not found or not assigned to you");
            if (order.Status != "InProgress") throw new InvalidOperationException("Order is not in progress");

            var completed = await _washerRepository.CompleteOrderAsync(orderId, imageUrl);
            if (!completed) throw new InvalidOperationException("Failed to complete order");

            // Send notification
            await SendWorkCompletedNotification(order, washer);

            return "Order completed successfully";
        }

        public async Task<IEnumerable<object>> GetMyOrdersAsync(int userId)
        {
            var washer = await _washerRepository.GetWasherByUserIdAsync(userId);
            if (washer == null) throw new KeyNotFoundException("Washer not found");

            return await _washerRepository.GetWasherOrdersAsync(washer.Id);
        }

        private Receipt GenerateReceipt(Order order)
        {
            var receiptNumber = $"GCW-{DateTime.UtcNow:yyyyMMdd}-{order.Id:D6}";
            var addonsDetails = order.OrderAddons?.Any() == true 
                ? string.Join(", ", order.OrderAddons.Select(oa => oa.Addon.Name))
                : "None";

            var details = $"Service: {order.WashPackage.Name}\nVehicle: {order.Vehicle.Make} {order.Vehicle.Model} ({order.Vehicle.LicensePlate})\nAddons: {addonsDetails}\nScheduled: {order.ScheduledTime:yyyy-MM-dd HH:mm}";

            return new Receipt
            {
                OrderId = order.Id,
                ReceiptNumber = receiptNumber,
                Details = details,
                Total = order.TotalAmount,
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private async Task SendOrderAcceptedNotification(Order order, Washer washer)
        {
            var subject = "Order Accepted - Green Car Wash";
            var body = $@"
                <h2>Your Order Has Been Accepted!</h2>
                <p>Dear {order.Customer.FullName},</p>
                <p>Great news! Your car wash order has been accepted.</p>
                
                <h3>Order Details:</h3>
                <ul>
                    <li><strong>Order ID:</strong> {order.Id}</li>
                    <li><strong>Service:</strong> {order.WashPackage.Name}</li>
                    <li><strong>Vehicle:</strong> {order.Vehicle.Make} {order.Vehicle.Model} ({order.Vehicle.LicensePlate})</li>
                    <li><strong>Scheduled Time:</strong> {order.ScheduledTime:yyyy-MM-dd HH:mm}</li>
                    <li><strong>Total Amount:</strong> ${order.TotalAmount:F2}</li>
                </ul>
                
                <p>Thank you for choosing Green Car Wash!</p>
            ";

            await _emailService.SendEmailAsync(order.Customer.Email, subject, body);
        }

        private async Task SendWorkStartedNotification(Order order, Washer washer)
        {
            var subject = "Work Started - Green Car Wash";
            var body = $@"
                <h2>Your Car Wash Service Has Started!</h2>
                <p>Dear {order.Customer.FullName},</p>
                <p>Your washer has started working on your vehicle.</p>
                
                <h3>Service Details:</h3>
                <ul>
                    <li><strong>Order ID:</strong> {order.Id}</li>
                    <li><strong>Vehicle:</strong> {order.Vehicle.Make} {order.Vehicle.Model} ({order.Vehicle.LicensePlate})</li>
                    <li><strong>Started At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm}</li>
                </ul>
                
                <p>Thank you for choosing Green Car Wash!</p>
            ";

            await _emailService.SendEmailAsync(order.Customer.Email, subject, body);
        }

        private async Task SendWorkCompletedNotification(Order order, Washer washer)
        {
            var subject = "Service Completed - Green Car Wash";
            var body = $@"
                <h2>Your Car Wash Service is Complete!</h2>
                <p>Dear {order.Customer.FullName},</p>
                <p>Great news! Your car wash service has been completed.</p>
                
                <h3>Service Summary:</h3>
                <ul>
                    <li><strong>Order ID:</strong> {order.Id}</li>
                    <li><strong>Vehicle:</strong> {order.Vehicle.Make} {order.Vehicle.Model} ({order.Vehicle.LicensePlate})</li>
                    <li><strong>Completed At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm}</li>
                    <li><strong>Total Amount:</strong> ${order.TotalAmount:F2}</li>
                </ul>
                
                <p>Thank you for choosing Green Car Wash!</p>
            ";

            await _emailService.SendEmailAsync(order.Customer.Email, subject, body);
        }
    }
}
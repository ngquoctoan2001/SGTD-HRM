using HRM.API.Hubs;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HRM.API.Services;

public class NotificationService : INotificationService
{
    private readonly HrmDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(HrmDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(int userId, string title, string message, string type, string link = "")
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Link = link,
            IsRead = false,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Push real-time notification
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
        {
            id = notification.Id,
            title = notification.Title,
            message = notification.Message,
            type = notification.Type,
            link = notification.Link,
            isRead = notification.IsRead,
            createdAt = notification.CreatedAt
        });
    }
}

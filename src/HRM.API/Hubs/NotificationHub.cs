using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HRM.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    // Hub specifically for pushing real-time notifications to connected clients
    public override async Task OnConnectedAsync()
    {
        // Add the user to a group based on their user ID or role to easily send targeted messages
        if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
        {
            var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var role = Context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
        {
            var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var role = Context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
}

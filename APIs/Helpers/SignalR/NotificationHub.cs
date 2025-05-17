using Microsoft.AspNetCore.SignalR;

namespace APIs.Helpers.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string Message)
        {
            await Clients.All.SendAsync("ReceiveNotification", Message);
        }
        public async Task SendNotificationToUser(string userId, string Message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", Message);
        }

    }
}

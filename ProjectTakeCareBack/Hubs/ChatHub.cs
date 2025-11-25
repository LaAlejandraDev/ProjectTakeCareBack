using Microsoft.AspNetCore.SignalR;
using ProjectTakeCareBack.Models;

namespace ProjectTakeCareBack.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, ChatMensaje message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyNewDateToUser(string userId, Cita cita)
        {
            await Clients.User(userId).SendAsync("NewDate", cita);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

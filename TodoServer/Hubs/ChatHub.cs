using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TodoShared;

namespace TodoServer.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        private const int MessageCacheLengths = 10;
        private static readonly List<User> Users = new();
        private static readonly Queue<Message> Messages = new();
        public override async Task OnConnectedAsync()
        {
            Users.Add(new User(Context.ConnectionId));
            await Clients.Caller.Connected();
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            var msg = new Message(user, message);
            Messages.Enqueue(msg);

            if (Messages.Count >= MessageCacheLengths)
                Messages.Dequeue();

            await Clients.All.TransferMessage(JsonSerializer.Serialize(msg));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var disconnectedUser = Users.FirstOrDefault(u => u.Id == Context.ConnectionId);

            if (disconnectedUser != null)
            {
                await Clients.All.UserDisconnected(disconnectedUser.Name);
                Users.Remove(disconnectedUser);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendUsersToCaller()
        {
            await Clients.Caller.SendAllUsers(new List<string>(Users.Select(e => e.Name)));
        }

        public async Task SetName(string username)
        {
            if (Users.Any(u => u.Name == username) || username == Constants.ServerName)
                await Clients.Caller.SetNameResult(false);
            else
            {
                var user = Users.FirstOrDefault(u => u.Id == Context.ConnectionId);
                if (user != null)
                {
                    user.Name = username;
                    await Clients.Caller.SetNameResult(true);
                    await Clients.Others.UserConnected(username);
                    await SendUsersToCaller();
                    await Clients.Caller.SendLastMessages(JsonSerializer.Serialize(new List<Message>(Messages)));
                }
            }
        }

    }
}

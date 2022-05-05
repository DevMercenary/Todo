using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoNotes.Data
{
    public interface IChatHub
    {
        Task Connected();
        Task TransferMessage(string messageJson);
        Task UserDisconnected(string name);
        Task UserConnected(string name);
        Task SendAllUsers(List<string> users);
        Task SetNameResult(bool isSet);
        Task SendLastMessages(string messagesJson);
    }
}

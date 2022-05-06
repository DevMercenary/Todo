namespace TodoShared
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

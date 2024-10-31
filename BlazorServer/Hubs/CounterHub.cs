using Microsoft.AspNetCore.SignalR;

namespace BlazorServer.Hubs;

public class CounterHub : Hub
{
    public Task SendMessage(string user, string message)
    {
        Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
        Clients.User("user").SendAsync("ReceiveMessage", user, message);
        Clients.Caller.SendAsync("ReceiveMessage", user, message);
        Clients.Others.SendAsync("ReceiveMessage", user, message);
        Clients.All.SendAsync("ReceiveMessage", user, message);
        Clients.AllExcept(new List<string> { "user1", "user2" }).SendAsync("ReceiveMessage", user, message);
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }


    public Task AddToTotal(string user, int amount)
    {
        return Clients.All.SendAsync("CounterIncrement", user, amount);
    }
}

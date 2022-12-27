using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Chat.Hubs
{
   // [Authorize]
    public class ChatHub : Hub
    {
        
        public async Task Send(string message, string to)
        {
            //var userName = Context.User.Identity.Name;

            //if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
            //    await Clients.User(Context.UserIdentifier).SendAsync("Receive", message, userName);
            await Clients.User(to).SendAsync("Receive", message);//, userName);
        }

    }
}

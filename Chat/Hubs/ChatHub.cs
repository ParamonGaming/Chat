using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            List<int?> ids = new();
            List<string> names = new();
            using (var db = new ApplicationContext())
            {
                // Retrieve user.
                var user = db.Users
                    .Include(u => u.Chanels)
                    .SingleOrDefault(u => u.UserName == Context.User.Identity.Name);

                // Add to each assigned group.
                foreach (var item in user.Chanels)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, item.GroupName);
                    ids.Add(item.Id);
                    names.Add(item.GroupName);
                }
            }
            await Clients.Caller.SendAsync("GetContacts", ids, names);
            await base.OnConnectedAsync();
        }
        public async Task Send(MessageData message, ChanelModel to, string user)
        {
            using (ApplicationContext db = new())
            {
                message.AuthorID = db.Users.Include(r => r.Role).FirstOrDefault(u => u.UserName == user).Id;
                message.ChanelID = to.Id;
                db.Messages.Add(message);
                db.SaveChanges();
            }
            await Clients.Group(to.GroupName).SendAsync("Receive", message);
        }
    }
}

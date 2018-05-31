using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.FlightSearch.Hubs
{
    public class FlightSearchHub : Hub
    {
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        public async Task Search(string request)
        {
            var searchResult = "test"; ;
           await Clients.Caller.SendAsync("ReceiveSearchResult", searchResult);
        }

        //public Task SendMessageToGroups(string message)
        //{
        //    List<string> groups = new List<string>() { "SignalR Users" };
        //    return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}

using DND.Common.Controllers.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DND.Common.SignalRHubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Notifications)]
    public class ApiNotificationHub<TDto> : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var roles = Context.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                       .Select(c => c.Value)
                       .ToList();

            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var roles = Context.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                     .Select(c => c.Value)
                     .ToList();

            foreach (var role in roles)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    public static class ApiNotificationHubServerExtensions
    {
        public static async Task Created<TDto>(this IHubContext<ApiNotificationHub<TDto>> hubContext, object dto)
        {
            await hubContext.Clients.All.SendAsync("Created", dto);
        }

        public static async Task Updated<TDto>(this IHubContext<ApiNotificationHub<TDto>> hubContext, object dto)
        {
            await hubContext.Clients.All.SendAsync("Updated", dto);
        }

        public static async Task Deleted<TDto>(this IHubContext<ApiNotificationHub<TDto>> hubContext, object id)
        {
            await hubContext.Clients.All.SendAsync("Deleted", id);
        }
    }
}

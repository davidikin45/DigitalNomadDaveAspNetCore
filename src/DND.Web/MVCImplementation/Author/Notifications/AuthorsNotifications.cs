using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Author.Notifications
{
    public class AuthorsNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<AuthorDto>>("/api/authors/notifications");
        }
    }
}

using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Blog.Notifications
{
    public class BlogPostsNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<BlogPostDto>>("/api/blog-posts/notifications");
        }
    }
}

using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Category.Notifications
{
    public class CategoriesNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<CategoryDto>>("/api/categories/notifications");
        }
    }
}

using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Domain.CMS.ContentHtmls.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.ContentHtml.Notifications
{
    public class ContentHtmlsNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<ContentHtmlDto>>("/api/content-htmls/notifications");
        }
    }
}

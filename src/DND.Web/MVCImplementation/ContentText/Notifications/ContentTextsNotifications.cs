using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Domain.CMS.ContentTexts.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.ContentText.Notifications
{
    public class ContentTextsNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<ContentTextDto>>("/api/content-texts/notifications");
        }
    }
}

﻿using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Domain.CMS.Testimonials.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Testimonial.Notifications
{
    public class TestimonialsNotifications
    {
        public static void MapHub(HubRouteBuilder routes)
        {
            routes.MapHub<ApiNotificationHub<TestimonialDto>>("/api/testimonials/notifications");
        }
    }
}

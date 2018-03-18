using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using DND.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class LinkExtensions
    {      
        public static string BuildUrlFromExpression<TController>(this IHtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            return LinkBuilder.BuildUrlFromExpression(helper.ViewContext.HttpContext, helper.ViewContext.RouteData.Values, helper.ViewContext.RouteData.Routers.OfType<RouteCollection>().FirstOrDefault(), action);
        }

    }
}

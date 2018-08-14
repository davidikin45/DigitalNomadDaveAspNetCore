using DND.Common.Helpers;
using DND.Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq.Expressions;

namespace DND.Common.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Content(this IUrlHelper urlHelper, string path, bool toAbsolute)
        {
            var absoluteVirtual = urlHelper.Content(path);

            if (!toAbsolute)
            {
                return absoluteVirtual;
            }
            else
            {
                var siteUrl = ConfigurationManager.AppSettings("SiteUrl");
                if (string.IsNullOrWhiteSpace(siteUrl))
                {
                    siteUrl = urlHelper.ActionContext.HttpContext.Request.Host.ToUriComponent();
                }

                var absoluteUri = string.Concat(
                        urlHelper.ActionContext.HttpContext.Request.Scheme,
                        "://",
                       siteUrl,
                        urlHelper.ActionContext.HttpContext.Request.PathBase.ToUriComponent(),
                        absoluteVirtual);
                return absoluteUri;
            }
        }

        public static string Content(this IUrlHelper urlHelper, string folderId, string path, bool toAbsolute)
        {
            var physicalPath = Server.GetWwwFolderPhysicalPathById(folderId) + path;
            string absoluteVirtual = physicalPath.GetAbsoluteVirtualPath();

            Uri url = new Uri(new Uri(ConfigurationManager.AppSettings("SiteUrl")), absoluteVirtual);

            return toAbsolute ? url.AbsoluteUri : absoluteVirtual;
        }

        public static string AbsoluteRouteUrl(
            this IUrlHelper urlHelper,
            string routeName,
            object routeValues = null)
        {
            string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }

        public static string AbsoluteUrl<TController>(this IUrlHelper url, Expression<Action<TController>> expression, bool passRouteValues = true) where TController : Controller
        {
            string absoluteUrl = "";
            var routeValues = ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            if (passRouteValues)
            {
                absoluteUrl = AbsoluteUrl(url, routeValues["Action"].ToString(), routeValues["Controller"].ToString(), routeValues);
            }
            else
            {
                absoluteUrl = AbsoluteUrl(url, routeValues["Action"].ToString(), routeValues["Controller"].ToString());
            }

            return absoluteUrl;
        }

        public static string AbsoluteUrl(this IUrlHelper url, string actionName, string controllerName, object routeValues)
        {
            return AbsoluteUrl(url, actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        public static string AbsoluteUrl(this IUrlHelper url, string actionName, string controllerName, RouteValueDictionary routeValues = null)
        {
            var absoluteUrl = "";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings("SiteUrl")))
            {
                absoluteUrl = string.Format("{0}{1}", ConfigurationManager.AppSettings("SiteUrl"), url.Action(actionName, controllerName, routeValues));
            }
            else
            {
                //The trick here is that once you specify the protocol/scheme when calling any routing method, you get an absolute URL. I recommend this one when possible, but you also have the more generic way in the first example in case you need it.
                absoluteUrl = url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme).ToString();
            }
            return absoluteUrl;
        }

        public static string Action<TController>(this IUrlHelper url, Expression<Action<TController>> expression) where TController : Controller
        {
            var routeValues = ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            return url.Action(routeValues["Action"].ToString(), routeValues["Controller"].ToString(), routeValues);
        }

        public static string RouteUrl<TController>(this IUrlHelper url, Expression<Action<TController>> expression) where TController : Controller
        {
            var routeValues = ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            return url.RouteUrl(routeValues);
        }
    }
}

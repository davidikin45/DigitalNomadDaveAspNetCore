using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Helpers
{
    public static class LinkBuilder
    {     
        public static string BuildUrlFromExpression<TController>(HttpContext httpContext, RouteValueDictionary ambientValues, RouteCollection routeCollection, Expression<Action<TController>> action) where TController : Controller
        {
            RouteValueDictionary newRouteValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            VirtualPathContext context = new VirtualPathContext(httpContext, ambientValues, newRouteValues);
            VirtualPathData vpd = routeCollection.GetVirtualPath(context);
            return (vpd == null) ? null : vpd.VirtualPath;
        }

    }
}

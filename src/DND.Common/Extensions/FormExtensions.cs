using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class FormExtensions
    {
       
        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            return BeginForm(helper, action, FormMethod.Post, null);
        }
     
        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action, FormMethod method) where TController : Controller
        {
            return BeginForm(helper, action, method, null);
        }
    
        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action, FormMethod method, object htmlAttributes) where TController : Controller
        {
            return BeginForm(helper, action, method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
    
        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action, FormMethod method, IDictionary<string, object> htmlAttributes) where TController : Controller
        {
            TagBuilder tagBuilder = new TagBuilder("form");
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            tagBuilder.MergeAttributes(htmlAttributes);
            string formAction = helper.BuildUrlFromExpression(action);
            tagBuilder.MergeAttribute("action", formAction);
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method));

            tagBuilder.WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

            return new MvcForm(helper.ViewContext, HtmlEncoder.Default);
        }
    }
}

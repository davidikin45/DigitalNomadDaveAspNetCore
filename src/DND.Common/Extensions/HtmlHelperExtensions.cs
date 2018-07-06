﻿using HtmlTags;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using DND.Common.APIs;
using DND.Common.Infrastructure;
using DND.Common.Extensions;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.Repository;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static T GetInstance<T>(this IHtmlHelper html)
        {
            return (T)html.ViewContext.HttpContext.RequestServices.GetService(typeof(T));
        }

        public static IFileSystemGenericRepositoryFactory FileSystemGenericRepositoryFactory(this IHtmlHelper html)
        {
            var repos = html.GetInstance<IFileSystemGenericRepositoryFactory>();
            return repos;
        }

        public static IBaseDbContext Database(this IHtmlHelper html)
        {
            var dbContext = html.GetInstance<IDbContextFactory>().CreateDefault();
            return dbContext;
        }

        public static TIDbContext Database<TIDbContext>(this IHtmlHelper html) where TIDbContext : IBaseDbContext
        {
            var dbContext = html.GetInstance<IDbContextFactory>().Create<TIDbContext>();
            return dbContext;
        }

        public static HtmlString ClientIdFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return new HtmlString(
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
        }

        public static HtmlString Grid(this IHtmlHelper<dynamic> html, Boolean details, Boolean edit, Boolean delete, Boolean sorting)
        {
            var table = new TagBuilder("table");
            table.AddCssClass("table");
            table.AddCssClass("table-striped");

            var thead = new TagBuilder("thead");

            var theadTr = new TagBuilder("tr");

            Boolean hasActions = (details || edit || delete);

            foreach (var prop in html.ViewData.ModelMetadata().Properties
            .Where(p => p.ShowForDisplay && (!p.AdditionalValues.ContainsKey("ShowForGrid") || (Boolean)p.AdditionalValues["ShowForGrid"])))
            {
                var th = new TagBuilder("th");

                var orderType = OrderByType.Descending;
                if (html.ViewBag.OrderColumn.ToLower() == prop.PropertyName.ToLower() && html.ViewBag.OrderType != OrderByType.Ascending)
                {
                    orderType = OrderByType.Ascending;
                }

                string linkText = ModelHelperExtensions.DisplayName(html.ViewData.Model, prop.PropertyName).ToString();
                string link;

                Boolean enableSort = sorting;
                if (prop.AdditionalValues.ContainsKey("AllowSortForGrid"))
                {
                    enableSort = (Boolean)prop.AdditionalValues["AllowSortForGrid"];
                }

                if (enableSort)
                {
                    link = html.ActionLink(linkText, "Index", new { page = html.ViewBag.Page, pageSize = html.ViewBag.PageSize, search = html.ViewBag.Search, orderColumn = prop.PropertyName, orderType = orderType }).Render();
                }
                else
                {
                    link = linkText;
                }

                // th.InnerHtml = ModelHelperExtensions.DisplayName(html.ViewData.Model, prop.PropertyName).ToString();
                th.InnerHtml.AppendHtml(link);

                theadTr.InnerHtml.AppendHtml(th);
            }

            if (hasActions)
            {
                var thActions = new TagBuilder("th");
                theadTr.InnerHtml.AppendHtml(thActions);
            }

            thead.InnerHtml.AppendHtml(theadTr);

            table.InnerHtml.AppendHtml(thead);

            var tbody = new TagBuilder("tbody");

            foreach (var item in html.ViewData.Model)
            {
                var tbodytr = new TagBuilder("tr");

                foreach (var prop in html.ViewData.ModelMetadata().Properties
               .Where(p => p.ShowForDisplay && (!p.AdditionalValues.ContainsKey("ShowForGrid") || (Boolean)p.AdditionalValues["ShowForGrid"])))
                {
                    var td = new TagBuilder("td");

                    if (prop.AdditionalValues.ContainsKey("DropdownModelType"))
                    {
                        HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, ModelHelperExtensions.Display(html, item, prop.PropertyName));
                    }
                    else if (prop.ModelType == typeof(FileInfo))
                    {
                        HtmlContentBuilderExtensions.SetHtmlContent(td.InnerHtml, ModelHelperExtensions.Display(html, item, prop.PropertyName));
                    }
                    else if (prop.ModelType == typeof(DbGeography))
                    {
                        var model = (DbGeography)item.GetType().GetProperty(prop.PropertyName).GetValue(item, null);
                        if (model != null && model.Latitude.HasValue && model.Longitude.HasValue)
                        {
                            string value = model.Latitude.Value.ToString("G", CultureInfo.InvariantCulture) + "," + model.Longitude.Value.ToString("G", CultureInfo.InvariantCulture);
                            td.InnerHtml.Append(value);
                        }
                    }
                    else
                    {
                        string value = ModelHelperExtensions.DisplayTextSimple(html, item, prop.PropertyName).ToString();
                        td.InnerHtml.Append(value.Truncate(70));
                    }

                    tbodytr.InnerHtml.AppendHtml(td);
                }

                if (hasActions)
                {
                    var tdActions = new TagBuilder("td");

                    if (details)
                    {
                        tdActions.InnerHtml.AppendHtml(html.ActionLink("Details", "Details", new { id = item.Id }));
                        if (edit || delete)
                        {
                            tdActions.InnerHtml.Append(" | ");
                        }
                    }

                    if (edit)
                    {
                        tdActions.InnerHtml.AppendHtml(html.ActionLink("Edit", "Edit", new { id = item.Id }));
                        if (delete)
                        {
                            tdActions.InnerHtml.Append(" | ");
                        }
                    }

                    if (delete)
                    {
                        tdActions.InnerHtml.AppendHtml(html.ActionLink("Delete", "Delete", new { id = item.Id }));
                    }

                    tbodytr.InnerHtml.AppendHtml(tdActions);
                }

                tbody.InnerHtml.AppendHtml(tbodytr);
            }

            table.InnerHtml.AppendHtml(tbody);

            return new HtmlString(table.Render());
        }

        public static string Controller(this IHtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ViewContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action(this IHtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ViewContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }

        public static HtmlString BootstrapPager(this IHtmlHelper html, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5)
        {
            if (totalItems <= 0)
            {
                return HtmlString.Empty;
            }
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }

            var nav = new TagBuilder("nav");
            nav.AddCssClass("pagination-nav");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");
            ul.InnerHtml.AppendHtml(AddLink(1, action, currentPageIndex == 1, "disabled", "<<", "First Page", false));
            ul.InnerHtml.AppendHtml(AddLink(currentPageIndex - 1, action, !hasPreviousPage, "disabled", "<", "Previous Page", false));
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml.AppendHtml(AddLink(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString(), false));
            }
            ul.InnerHtml.AppendHtml(AddLink(currentPageIndex + 1, action, !hasNextPage, "disabled", ">", "Next Page", true));
            ul.InnerHtml.AppendHtml(AddLink(totalPages, action, currentPageIndex == totalPages, "disabled", ">>", "Last Page", false));

            nav.InnerHtml.AppendHtml(ul);

          
            return new HtmlString(nav.Render());
        }

        //@Html.BootstrapPager(pageIndex, index => Url.Action("Index", "Product", new { pageIndex = index }), Model.TotalCount, numberOfLinks: 10)
        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, bool nextPage)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            li.MergeAttribute("title", tooltip);
            if (condition)
            {
                li.AddCssClass(classToAdd);
            }
            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            if (nextPage && !condition)
            {
                a.AddCssClass("pagination__next");
            }
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.InnerHtml.Append(linkText);
            li.InnerHtml.AppendHtml(a);
            return li;
        }

        public static HtmlString DisqusCommentCount(this IHtmlHelper helper, string path)
        {
            HtmlTag a = new HtmlTag("a");
            a.Attr("data-disqus-identifier", path);

            return new HtmlString(a.ToString());
        }

        public static HtmlString DisqusCommentCountScript(this IHtmlHelper helper, string disqusShortname)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var disqus_shortname = '" + disqusShortname + "';");

            sb.AppendLine("(function () {");
            sb.AppendLine("var s = document.createElement('script');");
            sb.AppendLine("s.async = true;");
            sb.AppendLine("s.src = 'http://' + disqus_shortname + '.disqus.com/count.js';");
            sb.AppendLine("(document.getElementsByTagName('HEAD')[0] || document.getElementsByTagName('BODY')[0]).appendChild(s);");
            sb.AppendLine("}");
            sb.AppendLine("());");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString FacebookCommentsThread(this IHtmlHelper helper, string siteUrl, string path, string title)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag div = new HtmlTag("div");
            div.AddClass("fb-comments");
            div.Attr("data-href", url);
            div.Attr("data-numposts", "10");
            div.Attr("width", "100%");
            /*div.Attr("data-order-by", "social"); reverse_time /*/

            return new HtmlString(div.ToString());
        }

        public static HtmlString FacebookCommentCount(this IHtmlHelper helper, string siteUrl, string path)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag span = new HtmlTag("span");
            span.AddClass("fb-comments-count");
            span.Attr("data-href", url);

            return new HtmlString(span.ToString());
        }

        //ideally right after the opening<body> tag.
        public static HtmlString FacebookCommentsScript(this IHtmlHelper helper, string appid)
        {
            HtmlTag div = new HtmlTag("div");
            div.Id("fb-root");

            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" (function(d, s, id) { ");

            sb.AppendLine(" var js, fjs = d.getElementsByTagName(s)[0]; ");
            sb.AppendLine(" if (d.getElementById(id)) return; ");
            sb.AppendLine(" js = d.createElement(s); js.id = id; ");
            sb.AppendLine(" js.src = '//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.9&appId=" + appid + "' ");
            sb.AppendLine(" fjs.parentNode.insertBefore(js, fjs); ");

            sb.AppendLine(" } ");
            sb.AppendLine(" (document, 'script', 'facebook-jssdk')); ");

            script.AppendHtml(sb.ToString());

            return new HtmlString(div.ToString() + script.ToString());
        }

        public static HtmlString GoogleAnalyticsScript(this IHtmlHelper helper, string trackingId)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){ ");
            sb.AppendLine(" (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o), ");
            sb.AppendLine(" m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m) ");
            sb.AppendLine(" })(window,document,'script','https://www.google-analytics.com/analytics.js','ga'); ");
            sb.AppendLine(" ga('create', '" + trackingId + "', 'auto'); ");
            sb.AppendLine(" ga('send', 'pageview'); ");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString GoogleAdSenseScript(this IHtmlHelper helper, string id)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");
            script.Attr("async", "");
            script.Attr("src", "//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js");

            HtmlTag script2 = new HtmlTag("script");
            script2.Attr("type", "text/javascript");

            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine(" (adsbygoogle = window.adsbygoogle || []).push({ ");
            sb2.AppendLine(" google_ad_client: '" + id + "', ");
            sb2.AppendLine(" enable_page_level_ads: false ");
            sb2.AppendLine(" }); ");

            script2.AppendHtml(sb2.ToString());

            return new HtmlString(script.ToString() + script2.ToString());
        }

        public static HtmlString DisqusThread(this IHtmlHelper helper, string disqusShortname, string siteUrl, string path, string title)
        {
            HtmlTag div = new HtmlTag("div");
            div.Id("disqus_thread");

            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            var url = string.Format("{0}{1}", siteUrl, path);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var disqus_shortname = '" + disqusShortname + "';");

            sb.AppendLine("var disqus_config = function() {");
            sb.AppendLine("this.page.url = '" + url + "'");
            sb.AppendLine("this.page.identifier = '" + path + "'");
            sb.AppendLine("this.page.title = '" + title + "'");
            sb.AppendLine("};");

            sb.AppendLine("(function() {");
            sb.AppendLine("var dsq = document.createElement('script');");
            sb.AppendLine("dsq.type = 'text/javascript';");
            sb.AppendLine("dsq.async = false;");
            sb.AppendLine("dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';");
            sb.AppendLine("(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);");
            sb.AppendLine("})();");

            script.AppendHtml(sb.ToString());

            HtmlTag noscript = new HtmlTag("noscript");
            noscript.AppendHtml("Please enable JavaScript to view the <a href=\"http://disqus.com/?ref_noscript\"> comments powered by Disqus.</a>");

            HtmlTag aDisqus = new HtmlTag("a");
            aDisqus.Attr("href", "http://disqus.com");
            aDisqus.AddClass("dsq-brlink");
            aDisqus.AppendHtml("blog comments powered by<span class=\"logo - disqus\">Disqus</span>");

            var finalHTML = div.ToString() + Environment.NewLine + script.ToString() + Environment.NewLine + noscript.ToString() + Environment.NewLine + aDisqus.ToString() + DisqusCommentCountScript(helper, disqusShortname).ToString();

            return new HtmlString(finalHTML);
        }

        public static HtmlString AddThisLinks(this IHtmlHelper helper, string siteUrl, string path, string title, string description, string imageUrl)
        {
            var url = string.Format("{0}{1}", siteUrl, path);

            HtmlTag div = new HtmlTag("div");
            div.Attr("class", "addthis_inline_share_toolbox_p3ki");
            div.Attr("data-url", url);
            div.Attr("data-title", title);
            div.Attr("data-description", description);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                div.Attr("data-media", imageUrl);
            }

            return new HtmlString(div.ToString());
        }

        public static HtmlString AddThisRelatedPosts(this IHtmlHelper helper)
        {

            HtmlTag div = new HtmlTag("div");
            div.Attr("class", "addthis_relatedposts_inline");

            return new HtmlString(div.ToString());
        }

        public static HtmlString ReturnToTop(this IHtmlHelper helper)
        {
            HtmlTag link = new HtmlTag("a");
            link.Attr("href", "javascript:");
            link.Id("return-to-top");
            link.AppendHtml(@"<i class=""fa fa-chevron-up""></i>");

            return new HtmlString(link.ToString());
        }

        public static HtmlString AddThisScript(this IHtmlHelper helper, string pubid)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("src", "https://s7.addthis.com/js/300/addthis_widget.js#pubid=" + pubid);
            script.Attr("type", "text/javascript");

            return new HtmlString(script.ToString());
        }

        public static HtmlString DeferredIFrameLoad(this IHtmlHelper helper)
        {
            HtmlTag script = new HtmlTag("script");
            script.Attr("type", "text/javascript");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function init() ");
            sb.AppendLine("{");
            sb.AppendLine("var vidDefer = document.getElementsByTagName('iframe');");
            sb.AppendLine("for (var i = 0; i < vidDefer.length; i++)");
            sb.AppendLine("{");
            sb.AppendLine("if (vidDefer[i].getAttribute('data-src'))");
            sb.AppendLine("{");
            sb.AppendLine("vidDefer[i].setAttribute('src', vidDefer[i].getAttribute('data-src'));");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("window.onload = init;");

            script.AppendHtml(sb.ToString());

            return new HtmlString(script.ToString());
        }

        public static HtmlString GoogleFontAsync(this IHtmlHelper helper, List<string> fonts, bool regular = true, bool bold = false, bool black = false, bool italic = false, bool boldItalic = false, Boolean hideTextWhileLoading = true, int timeout = 5000)
        {
            var html = Google.Font.GetFontScriptAsync(fonts, regular, bold, black, italic, boldItalic, hideTextWhileLoading, timeout);
            return new HtmlString(html);
        }

        public static HtmlString GetFontBodyCSSAsync(this IHtmlHelper helper, string font)
        {
            return new HtmlString(Google.Font.GetFontBodyCSSAsync(font));
        }

        public static HtmlString GetFontNavBarCSSAsync(this IHtmlHelper helper, string font, string styleCSS)
        {
            return new HtmlString(Google.Font.GetFontNavBarCSSAsync(font, styleCSS));
        }

        public static HtmlString GoogleFont(this IHtmlHelper helper, string font, string styleCSS, bool bodyFont, bool navBarFont, bool regular = true, bool bold = false, bool italic = false, bool boldItalic = false)
        {
            var html = Google.Font.GetFontLink(font, regular, bold, italic, boldItalic);
            if (bodyFont)
            {
                html += Environment.NewLine + Google.Font.GetFontBodyCSS(font);
            }

            if (navBarFont)
            {
                html += Environment.NewLine + Google.Font.GetFontNavBarCSS(font, styleCSS);
            }

            return new HtmlString(html);
        }

        public static HtmlString GoogleFontEffects(this IHtmlHelper helper, string[] effects)
        {
            var html = Google.Font.GetEffectsLink(effects);
            return new HtmlString(html);
        }

        public static IHtmlContent Action<TController>(this IHtmlHelper helper, Expression<Action<TController>> expression, Boolean passRouteValues = false) where TController : Controller
        {
            var controllerName = typeof(TController).Name;
            var routeValues = DND.Common.Helpers.ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            if (passRouteValues)
            {
                return helper.ActionLink(((MethodCallExpression)expression.Body).Method.Name, controllerName.Substring(0, controllerName.Length - "Controller".Length), routeValues);
            }
            else
            {
                return helper.ActionLink(((MethodCallExpression)expression.Body).Method.Name, controllerName.Substring(0, controllerName.Length - "Controller".Length));
            }

        }

        public static HtmlString MenuLink<TController>(this IHtmlHelper helper, Expression<Action<TController>> expression, string linkText, string classNames, string iconClass = "", string content = "") where TController : Controller
        {
            var routeData = helper.ViewContext.RouteData.Values;
            var currentController = routeData["controller"];
            var currentAction = routeData["action"];

            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            var actionName = ((MethodCallExpression)expression.Body).Method.Name;

            if (String.Equals(actionName, currentAction as string,
                      StringComparison.OrdinalIgnoreCase)
                &&
               String.Equals(controllerName, currentController as string,
                       StringComparison.OrdinalIgnoreCase))

            {
                classNames = classNames + " active";

                HtmlString activeLink = null;

                if (!string.IsNullOrEmpty(iconClass))
                {
                    activeLink = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}""><i class=""fa {3}""></i></a>", helper.Url().Action<TController>(expression), classNames, linkText, iconClass));
                }
                else if (!string.IsNullOrEmpty(content))
                {
                    activeLink = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}"">{3}{2}</a>", helper.Url().Action<TController>(expression), classNames, linkText, content));
                }
                else
                {
                    activeLink = new HtmlString(helper.ActionLink<TController>(expression, linkText,
                    new { @class = classNames, title = linkText }
                    ).Render());
                }

                return activeLink;

            }

            HtmlString link = null;

            if (!string.IsNullOrEmpty(iconClass))
            {
                link = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}""><i class=""fa {3}""></i></a>", helper.Url().Action<TController>(expression), classNames, linkText, iconClass));
            }
            else if (!string.IsNullOrEmpty(content))
            {
                link = new HtmlString(string.Format(@"<a href=""{0}"" class=""{1}"" title=""{2}"">{3}{2}</a>", helper.Url().Action<TController>(expression), classNames, linkText, content));
            }
            else
            {
                link = new HtmlString(helper.ActionLink<TController>(expression, linkText,
                new { @class = classNames, title = linkText }
                ).Render());
            }

            return link;
        }

        public static IHtmlContent MenuLink(this IHtmlHelper helper, string controllerName, string actionName, string linkText, string classNames)
        {
            if (controllerName != "")
            {
                var routeData = helper.ViewContext.RouteData.Values;
                var currentController = routeData["controller"];
                var currentAction = routeData["action"];

                if (String.Equals(actionName, currentAction as string,
                          StringComparison.OrdinalIgnoreCase)
                    &&
                   String.Equals(controllerName, currentController as string,
                           StringComparison.OrdinalIgnoreCase))

                {
                    classNames = classNames + " active";
                    var activeLink = helper.ActionLink(linkText, actionName, controllerName, new { },
                        new { @class = classNames, title = linkText, itemprop = "url" }
                        );
                    return activeLink;

                }

                var link = helper.ActionLink(linkText, actionName, controllerName, new { },
                    new { @class = classNames, title = linkText, itemprop = "url" }
                    );
                return link;
            }
            else
            {
                return new HtmlString(string.Format("<a href='{0}' itemprop='url' class='{1}'>{2}</a>", actionName, classNames, linkText));
            }
        }

        public static UrlHelper Url(this IHtmlHelper html)
        {
            return new UrlHelper(html.ViewContext);
        }

        //https://daveaglick.com/posts/getting-an-htmlhelper-for-an-alternate-model-type
        public static HtmlHelper<TModel> For<TModel>(this IHtmlHelper helper) where TModel : class, new()
        {
            return For<TModel>(helper.ViewContext, helper.ViewData);
        }

        public static HtmlHelper<dynamic> For(this IHtmlHelper helper, dynamic model)
        {
            return For(helper.ViewContext, helper.ViewData, model);
        }

        public static HtmlHelper<TModel> For<TModel>(this IHtmlHelper helper, TModel model)
        {
            return For<TModel>(helper.ViewContext, helper.ViewData, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData) where TModel : class, new()
        {
            TModel model = new TModel();
            return For<TModel>(viewContext, viewData, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, TModel model)
        {
            var newViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };

            ViewContext newViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer,
               new HtmlHelperOptions());

            var helper = new HtmlHelper<TModel>(
                HttpContext.GetInstance<IHtmlGenerator>(),
                HttpContext.GetInstance<ICompositeViewEngine>(),
                HttpContext.GetInstance<IModelMetadataProvider>(),
                HttpContext.GetInstance<IViewBufferScope>(),
                HttpContext.GetInstance<HtmlEncoder>(),
                HttpContext.GetInstance<UrlEncoder>(), 
                HttpContext.GetInstance<ExpressionTextCache>());

            helper.Contextualize(newViewContext);

            return helper;
        }

        public static HtmlHelper<dynamic> For(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection, dynamic model)
        {
            var newViewData = new ViewDataDictionary<dynamic>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };

            ViewContext newViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer,
                new HtmlHelperOptions());

            var helper = new HtmlHelper<dynamic>(
                HttpContext.GetInstance<IHtmlGenerator>(),
                HttpContext.GetInstance<ICompositeViewEngine>(),
                HttpContext.GetInstance<IModelMetadataProvider>(),
                HttpContext.GetInstance<IViewBufferScope>(),
                HttpContext.GetInstance<HtmlEncoder>(),
                HttpContext.GetInstance<UrlEncoder>(),
                HttpContext.GetInstance<ExpressionTextCache>());

            helper.Contextualize(newViewContext);

            return helper;
        }

        public static IHtmlContent ActionLink<TController>(this IHtmlHelper html, Expression<Action<TController>> expression, string linkText, object htmlAttributes = null, Boolean passRouteValues = true) where TController : Controller
        {
            RouteValueDictionary routeValues = DND.Common.Helpers.ExpressionHelper.GetRouteValuesFromExpression<TController>(expression);
            IDictionary<string, object> htmlAttributesDict = null;

            if(htmlAttributes !=null )
            {
                htmlAttributesDict =(IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);
            }
            else
            {
                htmlAttributesDict = new Dictionary<string, object>();
            }

            if (passRouteValues)
            {
                return html.ActionLink(linkText, routeValues["Action"].ToString(), routeValues["Controller"].ToString(), routeValues, htmlAttributesDict);
            }
            else
            {
                return html.ActionLink(linkText, routeValues["Action"].ToString(), routeValues["Controller"].ToString(), new RouteValueDictionary(), htmlAttributesDict);
            }
        }

    }
}

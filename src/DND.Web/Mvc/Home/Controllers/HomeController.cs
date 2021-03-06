﻿using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Controllers;
using DND.Common.Extensions;
using DND.Common.Filters;
using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Repository;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Blog.Tags.Dtos;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Infrastructure.Constants;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Web.Mvc.Blog.Controllers;
using DND.Web.Mvc.Contact.Models;
using DND.Web.Mvc.Countries.Controllers;
using DND.Web.Mvc.Locations.Controllers;
using DND.Web.Mvc.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.Mvc.Home.Controllers
{
    [Route("")]
    //[LogAction]
    public class HomeController : MvcControllerHomeBase
    {
        private IBlogApplicationService _blogService;
        private ILocationApplicationService _locationService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        private readonly IMailingListApplicationService _mailingListService;

        public HomeController(IBlogApplicationService blogService, ILocationApplicationService locationService, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper, IEmailService emailService, IMailingListApplicationService mailingListService, AppSettings appSettings)
            : base(mapper, emailService, appSettings)
        {
            if (blogService == null) throw new ArgumentNullException("blogService");
            _blogService = blogService;
            _locationService = locationService;
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
            _mailingListService = mailingListService;
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("", Name = "home")] //Specifies that this is the default action for the entire application. Route: /
        public IActionResult Index()
        {
            //Validate ViewModel
            if (ModelState.IsValid)
            {
                try
                {

                }
                catch
                {
                    return HandleReadException();
                }
            }
            return View();
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index(MailingListDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mailingListService.Create(dto, Username);
                    return View("Index").WithSuccess(this, "Thankyou, your subscription was successful.");
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            //error
            return View("Index",dto);
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Contact" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("contact")]
        public override ActionResult Contact()
        {
            return base.Contact();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "WorkWithMe" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("work-with-me")]
        public ActionResult WorkWithMe()
        {
            ViewBag.Message = "Your application description page.";
            var u = Username;

            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "MyWebsite" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("my-website")]
        public ActionResult MyWebsite()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "About" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("about")]
        public ActionResult About()
        {
           // ResponseCachingCustomMiddleware.ClearResponseCache();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Resume" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("resume")]
        public ActionResult Resume()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "HelpFAQ" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("help-faq")]
        public ActionResult HelpFAQ()
        {
            ViewBag.Message = @"Your Help\FAQ page.";

            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "PrivacyPolicy" })]
        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("privacy-policy")]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Contact" })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contact")]
        public async Task<IActionResult> Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new EmailMessage();
                    message.Body = contact.Message;
                    message.IsHtml = true;
                    message.Subject = contact.Subject;
                    message.ReplyDisplayName = contact.Name;
                    message.ReplyEmail = contact.Email;

                    var result = await EmailService.SendEmailMessageToAdminAsync(message);

                    if (result.IsSuccess)
                    {
                        //Clears all post data
                        //https://stackoverflow.com/questions/1775170/asp-net-mvc-modelstate-clear
                        ViewData.ModelState.Clear();

                        return View().WithSuccess(this, Messages.MessageSentSuccessfully);
                    }
                    else
                    {
                        HandleUpdateException(result, null, true);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            if (contact.Message == null)
            {
                ViewData.ModelState.Clear();
            }

            return View(contact);
        }

        protected async override Task<IList<SitemapNode>> GetSitemapNodes(CancellationToken cancellationToken)
        {
            IList<SitemapNode> nodes = await base.GetSitemapNodes(cancellationToken);

            //Locations
            nodes.Add(
                 new SitemapNode()
                 {
                     Url = Url.AbsoluteUrl<LocationsController>(c => c.Index(1, 20, nameof(LocationDto.Name), "asc", ""), false),
                     Priority = 0.9
                 });

            //countries
            nodes.Add(
             new SitemapNode()
             {
                 Url = Url.AbsoluteUrl<CountriesController>(c => c.Index(1, 20, nameof(LocationDto.Name),"asc", ""), false),
                 Priority = 0.9
             });

            foreach (TagDto t in (await _blogService.TagApplicationService.GetAllAsync(cancellationToken, null, null, null)))
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = Url.AbsoluteUrl(nameof(BlogController.Tag), "Blog", new { tagSlug = t.UrlSlug }),
                       Frequency = SitemapFrequency.Weekly,
                       Priority = 0.8
                   });
            }

            foreach (CategoryDto c in (await _blogService.CategoryApplicationService.GetAsync(cancellationToken, c => c.Published, null, null)))
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = Url.AbsoluteUrl(nameof(BlogController.Category), "Blog", new { categorySlug = c.UrlSlug }),
                       Frequency = SitemapFrequency.Weekly,
                       Priority = 0.8
                   });
            }

            foreach (BlogPostDto p in (await _blogService.BlogPostApplicationService.GetPostsAsync(0, 200, cancellationToken)))
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = Url.AbsoluteUrl<BlogController>(c => c.Post(p.DateCreated.Year, p.DateCreated.Month, p.UrlSlug)),
                       Frequency = SitemapFrequency.Weekly,
                       Priority = 0.7
                   });
            }

            var repository = _fileSystemGenericRepositoryFactory.CreateFolderRepository(cancellationToken, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
            foreach (DirectoryInfo f in (await repository.GetAllAsync(LamdaHelper.GetOrderByFunc<DirectoryInfo>(nameof(DirectoryInfo.LastWriteTime), OrderByType.Descending), null, null)))
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = Url.AbsoluteUrl("Gallery", "Gallery", new { name = f.Name.ToSlug() }),
                       Frequency = SitemapFrequency.Weekly,
                       Priority = 0.7
                   });
            }

            foreach (LocationDto l in (await _locationService.GetAllAsync(cancellationToken, null, null, null)))
            {
                if (!string.IsNullOrEmpty(l.UrlSlug))
                {
                    nodes.Add(
                       new SitemapNode()
                       {
                           Url = Url.AbsoluteUrl<LocationsController>(lc => lc.Location(l.UrlSlug)),
                           Frequency = SitemapFrequency.Weekly,
                           Priority = 0.6
                       });
                }
            }

            return nodes;
        }

        protected async override Task<IEnumerable<System.ServiceModel.Syndication.SyndicationItem>> RSSItems(CancellationToken cancellationToken)
        {
            var posts = (await _blogService.BlogPostApplicationService.GetPostsAsync(0, 200, cancellationToken)).Select
            (
              p => new SyndicationItem
                  (
                      p.Title,
                      HtmlOutputHelper.RelativeToAbsoluteUrls(p.Description, AppSettings.SiteUrl),
                      new Uri(Url.AbsoluteUrl<BlogController>(c => c.Post(p.DateCreated.Year, p.DateCreated.Month, p.UrlSlug)))
                  )
                 
            ).ToList();

            return posts;
        }

        protected override string SiteTitle
        {
            get
            {
                return AppSettings.SiteTitle;
            }
        }

        protected override string SiteDescription
        {
            get
            {
                return AppSettings.SiteDescription;
            }

        }

        protected override string SiteUrl
        {
            get
            {
                return AppSettings.SiteUrl;
            }

        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Spa")]
        public IActionResult Spa()
        {
            return View();
        }
    }
}

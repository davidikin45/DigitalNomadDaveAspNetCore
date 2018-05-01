﻿using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Blog.Tags.Dtos;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Domain.Constants;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Implementation.Blog.Controllers;
using DND.Web.Implementation.Contact.Models;
using DND.Web.Implementation.Location.Controllers;
using DND.Web.Implementation.Shared.Models;
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

namespace DND.Web.Implementation.Home.Controllers
{
    [Route("")]
    //[LogAction]
    public class HomeController : BaseHomeController
    {
        private IBlogApplicationService _blogService;
        private ILocationApplicationService _locationService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;
        private readonly IMailingListApplicationService _mailingListService;

        public HomeController(IBlogApplicationService blogService, ILocationApplicationService locationService, IFileSystemRepositoryFactory fileSystemRepositoryFactory, IMapper mapper, IEmailService emailService, IMailingListApplicationService mailingListService, IConfiguration configuration)
            : base(mapper, emailService, configuration)
        {
            if (blogService == null) throw new ArgumentNullException("blogService");
            _blogService = blogService;
            _locationService = locationService;
            _fileSystemRepositoryFactory = fileSystemRepositoryFactory;
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
                catch (Exception ex)
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
                    _mailingListService.Create(dto);
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

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("contact")]
        public override ActionResult Contact()
        {
            return base.Contact();
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("work-with-me")]
        public ActionResult WorkWithMe()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("my-website")]
        public ActionResult MyWebsite()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("about")]
        public ActionResult About()
        {
           // ResponseCachingCustomMiddleware.ClearResponseCache();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("resume")]
        public ActionResult Resume()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("help-faq")]
        public ActionResult HelpFAQ()
        {
            ViewBag.Message = @"Your Help\FAQ page.";

            return View();
        }

        [HttpPost]
        [Route("contact")]
        public IActionResult Contact(ContactViewModel contact)
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

                    EmailService.SendEmailMessageToAdmin(message);

                    //Clears all post data
                    //https://stackoverflow.com/questions/1775170/asp-net-mvc-modelstate-clear
                    ViewData.ModelState.Clear();

                    return View().WithSuccess(this, "Thankyou, your message was sent successfully."); ;
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
                     Url = Url.AbsoluteUrl<LocationController>(c => c.Index(1, 20, nameof(LocationDto.Name), OrderByType.Ascending, ""), false),
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

            foreach (CategoryDto c in (await _blogService.CategoryApplicationService.GetAllAsync(cancellationToken, null, null, null)))
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

            var repository = _fileSystemRepositoryFactory.CreateFolderRepository(cancellationToken, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
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
                           Url = Url.AbsoluteUrl<LocationController>(lc => lc.Location(l.UrlSlug)),
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
                      HtmlOutputHelper.RelativeToAbsoluteUrls(p.Description, ConfigurationManager.AppSettings(Configuration, "SiteUrl")),
                      new Uri(Url.AbsoluteUrl<BlogController>(c => c.Post(p.DateCreated.Year, p.DateCreated.Month, p.UrlSlug)))
                  )
            );

            return posts;
        }

        protected override string SiteTitle
        {
            get
            {
                return ConfigurationManager.AppSettings(Configuration, "SiteTitle");
            }
        }

        protected override string SiteDescription
        {
            get
            {
                return ConfigurationManager.AppSettings(Configuration, "SiteDescription");
            }

        }

        protected override string SiteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings(Configuration, "SiteUrl");
            }

        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

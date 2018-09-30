using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Controllers;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Web.CMS.Mvc.MailingList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/cms/mailing-list")]
    public class AdminMailingListController : MvcControllerEntityAuthorizeBase<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListApplicationService>
    {
        public AdminMailingListController(IMailingListApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }

        [Route("email")]
        public virtual ActionResult Email()
        {
            var instance = new MailingListEmailViewModel();
            ViewBag.PageTitle = "Mailing List Email";
            ViewBag.Admin = Admin;
            return View("Email", instance);
        }

        // POST: Default/Create
        [HttpPost]
        [Route("email")]
        public virtual async Task<ActionResult> Email(MailingListEmailViewModel email)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var data = await Service.GetAllAsync(cts.Token);

                    List<EmailMessage> list = new List<EmailMessage>();

                    foreach(MailingListDto dto in data)
                    {
                        var message = new EmailMessage();
                        message.Body = email.Body;
                        message.IsHtml = true;
                        message.Subject = email.Subject;
                        message.ToEmail = dto.Email;
                        message.ToDisplayName = dto.Name;

                        list.Add(message);
                    }

                    EmailService.SendEmailMessages(list);

                    return RedirectToAction<AdminMailingListController>(c => c.Email()).WithSuccess(this, Messages.AddSuccessful);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            //error
            return View("Email", email);
        }
    }
}

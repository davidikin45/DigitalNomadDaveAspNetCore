using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Alerts;
using Solution.Base.Controllers;
using Solution.Base.Email;
using Solution.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/mailing-list")]
    public class AdminMailingListController : BaseEntityControllerAuthorize<MailingListDTO,IMailingListService>
    {
        public AdminMailingListController(IMailingListService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

        [Route("email")]
        public virtual ActionResult Email()
        {
            var instance = new MailingListEmail();
            ViewBag.PageTitle = "Mailing List Email";
            ViewBag.Admin = Admin;
            return View("Email", instance);
        }

        // POST: Default/Create
        [HttpPost]
        [Route("email")]
        public virtual async Task<ActionResult> Email(MailingListEmail email)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var data = await Service.GetAllAsync(cts.Token);

                    List<EmailMessage> list = new List<EmailMessage>();

                    foreach(MailingListDTO dto in data)
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

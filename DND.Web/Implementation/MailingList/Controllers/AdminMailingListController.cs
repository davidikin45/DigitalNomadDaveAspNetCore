﻿using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Implementation.MailingList.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/mailing-list")]
    public class AdminMailingListController : BaseEntityControllerAuthorize<MailingListDto, MailingListDto, MailingListDto, MailingListDto, IMailingListApplicationService>
    {
        public AdminMailingListController(IMailingListApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
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
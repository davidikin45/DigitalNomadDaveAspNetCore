using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using System;

namespace DND.Web.Controllers
{
    public class MailingListController : BaseController
    {
        private readonly IMailingListService _mailingListService;

        public MailingListController(IMailingListService mailingListService, IMapper mapper)
             : base(mapper)
        {
            _mailingListService = mailingListService;
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        //[ChildActionOnly]
        public PartialViewResult Submit()
        {
            var dto = new MailingListDTO();
            return PartialView("_MailingList", dto);
        }

        [HttpPost]
        public IActionResult Submit(MailingListDTO dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mailingListService.Create(dto);
                    return PartialView("_Thankyou", dto);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            //error
            return PartialView("_MailingList", dto);
        }
    }
}

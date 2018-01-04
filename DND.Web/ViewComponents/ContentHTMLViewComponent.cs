using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    [ViewComponent]
    public class ContentHTMLViewComponent : BaseViewComponent
    {
        private readonly IContentHtmlService Service;

        public ContentHTMLViewComponent(IContentHtmlService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            ContentHtmlDTO data = await Service.GetByIdAsync(id, cts.Token);

            return View(data);
        }
    }
}

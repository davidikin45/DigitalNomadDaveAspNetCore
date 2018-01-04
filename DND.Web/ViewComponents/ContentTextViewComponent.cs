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
    public class ContentTextViewComponent : BaseViewComponent
    {
        private readonly IContentTextService Service;

        public ContentTextViewComponent(IContentTextService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            ContentTextDTO data = await Service.GetByIdAsync(id, cts.Token);

            return View(data);
        }
    }
}

using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Helpers;
using DND.Common.ViewComponents;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    [ViewComponent]
    public class ContentHTMLViewComponent : BaseViewComponent
    {
        private readonly IContentHtmlApplicationService Service;

        public ContentHTMLViewComponent(IContentHtmlApplicationService service)
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

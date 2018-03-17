using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.ViewComponents;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public class ContentTextViewComponent : BaseViewComponent
    {
        private readonly IContentTextApplicationService Service;

        public ContentTextViewComponent(IContentTextApplicationService service)
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

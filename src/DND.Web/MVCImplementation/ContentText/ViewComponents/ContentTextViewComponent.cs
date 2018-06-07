using DND.Common.Helpers;
using DND.Common.ViewComponents;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.ContentText.ViewComponents
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

            ContentTextDto data = await Service.GetByIdAsync(id, cts.Token);

            return View(data);
        }
    }
}

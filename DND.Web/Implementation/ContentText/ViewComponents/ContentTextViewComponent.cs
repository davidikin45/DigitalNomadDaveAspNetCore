using DND.Common.Helpers;
using DND.Common.ViewComponents;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DND.Web.Implementation.ContentText.ViewComponents
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

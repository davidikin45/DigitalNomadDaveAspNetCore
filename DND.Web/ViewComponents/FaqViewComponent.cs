using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Helpers;
using DND.Common.Implementation.DTOs;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public class FaqViewComponent : BaseViewComponent
    {
        private readonly IFaqApplicationService Service;

        public FaqViewComponent(IFaqApplicationService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<FaqDTO> data = null;
            int total = 0;


            var dataTask = Service.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<FaqDTO>(nameof(FaqDTO.DateCreated), OrderByType.Ascending), null, null, null);
            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            data = dataTask.Result;
            total = totalTask.Result;


            var response = new WebApiPagedResponseDTO<FaqDTO>
            {
                Page = 1,
                PageSize = total,
                Records = total,
                Rows = data.ToList()
            };

            ViewBag.Page = 1;
            ViewBag.PageSize = total;

            return View(response);
        }
    }
}

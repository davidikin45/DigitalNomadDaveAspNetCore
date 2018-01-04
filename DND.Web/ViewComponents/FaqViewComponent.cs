using DND.Domain.Constants;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Web.Models.SidebarViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using Solution.Base.ViewComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public class FaqViewComponent : BaseViewComponent
    {
        private readonly IFaqService Service;

        public FaqViewComponent(IFaqService service)
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

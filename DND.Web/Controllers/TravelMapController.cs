using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Email;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Controllers
{
    [Route("travel-map")]
    public class TravelMapController : BaseController
	{
        private readonly ILocationApplicationService Service;

        public TravelMapController(ILocationApplicationService service, IMapper mapper, IEmailService emailService)
            :base(mapper, emailService)
		{
            Service = service;
        }

        [ResponseCache(CacheProfileName = "Cache24HourNoParams")]
        [Route("")]
		public async Task<ActionResult> Index()
		{
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = Service.GetAsync(cts.Token, l => l.ShowOnTravelMap == true, null, null, null);
                var totalTask = Service.GetCountAsync(cts.Token, l => l.ShowOnTravelMap);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDTO<LocationDTO>
                {
                    Page = 1,
                    PageSize = total,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = "",
                    OrderType = ""
                };

                ViewBag.Page = 1;
                ViewBag.PageSize = total;

                return View(response);
            }
            catch
            {
                return HandleReadException();
            }
        }
		
	}
}
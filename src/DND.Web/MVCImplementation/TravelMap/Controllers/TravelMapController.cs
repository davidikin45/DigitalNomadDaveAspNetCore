using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Common.Implementation.Dtos;
using DND.Domain.Blog.Locations.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.TravelMap.Controllers
{
    [Route("travel-map")]
    public class TravelMapController : BaseController
	{
        private readonly ILocationApplicationService Service;

        public TravelMapController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
            :base(mapper, emailService, configuration)
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

                var response = new WebApiPagedResponseDto<LocationDto>
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
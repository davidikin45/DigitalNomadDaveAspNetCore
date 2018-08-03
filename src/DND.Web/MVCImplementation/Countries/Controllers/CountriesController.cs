using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Filters;
using DND.Common.Helpers;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Repository;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Blog.Locations.Enums;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Countries.Controllers
{
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Countries" })]
    [Route("countries")]
    public class CountriesController : BaseController
    {
        private readonly ILocationApplicationService _locationService;

        public CountriesController(ILocationApplicationService locationService, IMapper mapper, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IEmailService emailService, IConfiguration configuration)
             : base(mapper, emailService, configuration)
        {
            _locationService = locationService;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string orderColumn = nameof(LocationDto.Name), string orderType = OrderByType.Ascending, string search = "")
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = _locationService.SearchAsync(cts.Token, LocationType.Country.ToString() + "&" + search, l => !string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug), LamdaHelper.GetOrderBy<LocationDto>(orderColumn, orderType), page - 1, pageSize);
                var totalTask = _locationService.GetSearchCountAsync(cts.Token, LocationType.Country.ToString() + "&" + search, l =>!string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug));

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<LocationDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                return View(response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }
    }
}

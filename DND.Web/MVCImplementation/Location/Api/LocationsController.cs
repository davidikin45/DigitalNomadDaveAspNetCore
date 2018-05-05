using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Location.Api
{
    [ApiVersion("1.0")]
    [Route("api/locations")]
    public class LocationsController : BaseEntityWebApiControllerAuthorize<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public LocationsController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}

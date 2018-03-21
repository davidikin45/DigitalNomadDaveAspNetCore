using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Location.Api
{
    [ApiVersion("1.0")]
    [Route("api/location")]
    public class LocationController : BaseEntityWebApiControllerAuthorize<LocationDto, LocationDto, LocationDto, LocationDto, ILocationApplicationService>
    {
        public LocationController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}

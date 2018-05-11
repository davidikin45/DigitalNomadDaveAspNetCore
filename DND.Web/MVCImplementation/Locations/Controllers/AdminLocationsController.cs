using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Locations.Controllers
{
    [Route("admin/locations")]
    public class AdminLocationsController : BaseEntityControllerAuthorize<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public AdminLocationsController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }

    }
}

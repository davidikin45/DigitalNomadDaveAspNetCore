using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Location.Controllers
{
    [Route("admin/location")]
    public class AdminLocationController : BaseEntityControllerAuthorize<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public AdminLocationController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }

    }
}

using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.Blog.Locations.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.Mvc.Locations.Controllers
{
    [Route("admin/blog/locations")]
    public class AdminLocationsController : MvcControllerEntityAuthorizeBase<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public AdminLocationsController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }

    }
}

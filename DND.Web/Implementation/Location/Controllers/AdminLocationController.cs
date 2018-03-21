using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Location.Controllers
{
    [Route("admin/location")]
    public class AdminLocationController : BaseEntityControllerAuthorize<LocationDto, LocationDto, LocationDto, LocationDto, ILocationApplicationService>
    {
        public AdminLocationController(ILocationApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

    }
}

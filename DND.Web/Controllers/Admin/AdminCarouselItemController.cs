using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/carousel-item")]
    public class AdminCarouselItemController : BaseEntityControllerAuthorize<CarouselItemDTO, ICarouselItemApplicationService>
    {
        public AdminCarouselItemController(ICarouselItemApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

    }
}

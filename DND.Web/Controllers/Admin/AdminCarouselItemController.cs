using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/carousel-item")]
    public class AdminCarouselItemController : BaseEntityControllerAuthorize<CarouselItemDTO, ICarouselItemService>
    {
        public AdminCarouselItemController(ICarouselItemService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

    }
}

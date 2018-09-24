using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.CarouselItem.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/carousel-items")]
    public class CarouselItemsController : ApiControllerEntityAuthorizeBase<CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, ICarouselItemApplicationService>
    {

        public CarouselItemsController(ICarouselItemApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }      
    }
}

using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/category")]
    public class CategoryController : BaseEntityWebApiControllerAuthorize<CategoryDTO,ICategoryService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logFactory">The log factory.</param>
        public CategoryController(ICategoryService service, IMapper mapper, IEmailService emailService)
            :base(service,mapper, emailService)
        {

        }      
    }
}

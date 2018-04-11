using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/categories")]
    public class CategoriesController : BaseEntityWebApiControllerAuthorize<CategoryDto, CategoryDto, CategoryDto, CategoryDto, ICategoryApplicationService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logFactory">The log factory.</param>
        public CategoriesController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }      
    }
}

using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Categories.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Blog.Mvc.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/blog/categories")]
    public class CategoriesController : ApiControllerEntityAuthorizeBase<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryApplicationService>
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

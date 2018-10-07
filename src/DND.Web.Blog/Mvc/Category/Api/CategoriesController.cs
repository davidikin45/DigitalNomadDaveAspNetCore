using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Categories.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Blog.Mvc.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/blog/categories")]
    public class CategoriesController : ApiControllerEntityAuthorizeBase<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryApplicationService>
    {
        public CategoriesController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
            : base("blog.categories.", service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {

        }      
    }
}

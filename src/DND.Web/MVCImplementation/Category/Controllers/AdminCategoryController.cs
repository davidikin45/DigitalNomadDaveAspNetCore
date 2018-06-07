using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Categories.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Category.Controllers
{
    [Route("admin/category")]
    public class AdminCategoryController : BaseEntityControllerAuthorize<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryApplicationService>
    {
        public AdminCategoryController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}

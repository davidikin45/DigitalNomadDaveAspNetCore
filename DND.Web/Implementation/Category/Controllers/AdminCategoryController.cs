using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.Controllers
{
    [Route("admin/category")]
    public class AdminCategoryController : BaseEntityControllerAuthorize<CategoryDto, CategoryDto, CategoryDto, CategoryDto, ICategoryApplicationService>
    {
        public AdminCategoryController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}

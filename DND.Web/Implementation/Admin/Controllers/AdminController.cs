using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers.Admin;

namespace DND.Web.Implementation.Admin.Controllers
{
    //[LayoutInjector("_LayoutAdmin")]
    [Route("admin")]
    public class AdminController : BaseAdminControllerAuthorize
    {

        public AdminController(IMapper mapper)
             : base(mapper)
        {
        }

    }
}
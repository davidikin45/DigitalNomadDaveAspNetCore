using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Admin;

namespace DND.Web.Controllers.Admin
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
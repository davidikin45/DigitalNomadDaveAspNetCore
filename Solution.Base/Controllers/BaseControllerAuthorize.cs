using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Solution.Base.Email;

namespace Solution.Base.Controllers
{
    [Authorize(Roles = "admin")]
    public abstract class BaseControllerAuthorize : BaseController
    {
        public BaseControllerAuthorize()
        {

        }

        public BaseControllerAuthorize(IMapper mapper = null, IEmailService emailService = null)
            :base(mapper, emailService)
        {
         
        }
    }
}

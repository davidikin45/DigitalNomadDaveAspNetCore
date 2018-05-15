using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DND.Common.Email;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Controllers
{
    [Authorize(Roles = "admin")]
    public abstract class BaseControllerAuthorize : BaseController
    {
        public BaseControllerAuthorize()
        {

        }

        public BaseControllerAuthorize(IMapper mapper = null, IEmailService emailService = null, IConfiguration configuration = null)
            :base(mapper, emailService, configuration)
        {
         
        }
    }
}

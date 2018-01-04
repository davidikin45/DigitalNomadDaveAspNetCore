using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Solution.Base.Email;

namespace Solution.Base.Controllers.Api
{


    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class BaseWebApiControllerAuthorize : BaseWebApiController
    {
        public BaseWebApiControllerAuthorize()
        {

        }

        public BaseWebApiControllerAuthorize(IMapper mapper = null, IEmailService emailService = null)
            :base(mapper, emailService)
        {
           
        }
    }
}


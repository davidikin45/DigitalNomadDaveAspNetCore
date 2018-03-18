using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/author")]
    public class AdminAuthorController : BaseEntityControllerAuthorize<AuthorDTO, IAuthorApplicationService>
    {
        public AdminAuthorController(IAuthorApplicationService service, IMapper mapper,IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}

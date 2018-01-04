using AutoMapper;
using DND.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Email;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/metadata")]
    public class AdminMetadataController : BaseJpegMetadataControllerAuthorize
    {
        public AdminMetadataController(IFileSystemRepositoryFactory fileSytemRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemRepositoryFactory, mapper, emailService)
        {

        }
    }
}

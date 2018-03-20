using AutoMapper;
using DND.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;

namespace DND.Web.Implementation.Admin.Controllers
{
    [Route("admin/file-metadata")]
    public class AdminFileMetadataController : BaseFileMetadataControllerAuthorize
    {
        public AdminFileMetadataController(IFileSystemRepositoryFactory fileSytemRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemRepositoryFactory, mapper, emailService)
        {

        }
    }
}

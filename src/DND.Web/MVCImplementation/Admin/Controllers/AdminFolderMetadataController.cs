using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;
using DND.Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.MVCImplementation.Admin.Controllers
{
    [Route("admin/folder-metadata")]
    public class AdminFolderMetadataController : BaseFolderMetadataControllerAuthorize
    {
        public AdminFolderMetadataController(IFileSystemGenericRepositoryFactory fileSytemGenericRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemGenericRepositoryFactory, mapper, emailService)
        {

        }
    }
}

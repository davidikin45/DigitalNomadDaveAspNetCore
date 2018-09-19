using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Email;
using DND.Common.Interfaces.Repository;
using DND.Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.MVCImplementation.Admin.Controllers
{
    [Route("admin/file-metadata")]
    public class AdminFileMetadataController : MvcControllerFileMetadataAuthorizeBase
    {
        public AdminFileMetadataController(IFileSystemGenericRepositoryFactory fileSytemGenericRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemGenericRepositoryFactory, mapper, emailService)
        {

        }
    }
}

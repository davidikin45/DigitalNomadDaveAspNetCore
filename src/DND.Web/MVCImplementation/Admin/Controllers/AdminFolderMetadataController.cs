﻿using AutoMapper;
using DND.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;

namespace DND.Web.MVCImplementation.Admin.Controllers
{
    [Route("admin/folder-metadata")]
    public class AdminFolderMetadataController : BaseFolderMetadataControllerAuthorize
    {
        public AdminFolderMetadataController(IFileSystemRepositoryFactory fileSytemRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemRepositoryFactory, mapper, emailService)
        {

        }
    }
}
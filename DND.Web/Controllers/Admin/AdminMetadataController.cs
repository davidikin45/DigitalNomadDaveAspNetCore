﻿using AutoMapper;
using DND.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;

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

﻿using AutoMapper;
using DND.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;

namespace DND.Web.MVCImplementation.Admin.Controllers
{
    [Route("admin/metadata")]
    public class AdminMetadataController : BaseJpegMetadataControllerAuthorize
    {
        public AdminMetadataController(IFileSystemGenericRepositoryFactory fileSytemGenericRepositoryFactory, IMapper mapper, IEmailService emailService)
             : base(Server.GetWwwFolderPhysicalPathById(Folders.Uploads), true, true, fileSytemGenericRepositoryFactory, mapper, emailService)
        {

        }
    }
}

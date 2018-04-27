﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.Projects.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Project.Api
{
    [ApiVersion("1.0")]
    [Route("api/projects")]
    public class ProjectsController : BaseEntityWebApiControllerAuthorize<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectApplicationService>
    {
        public ProjectsController(IProjectApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
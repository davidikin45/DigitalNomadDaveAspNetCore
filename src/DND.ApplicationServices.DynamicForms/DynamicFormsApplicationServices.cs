﻿using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Authorization;

namespace DND.ApplicationServices.DynamicForms
{
    public class DynamicFormsApplicationServices : ApplicationServiceBase, IDynamicFormsApplicationServices
    {
        public IFormApplicationService FormApplicationService { get; private set; }
        public IFormSubmissionApplicationService FormSubmissionApplicationService { get; private set; }
        public IFormSectionSubmissionApplicationService FormSectionSubmissionApplicationService { get; private set; }
        public ILookupTableApplicationService LookupTableApplicationService { get; private set; }
        public IQuestionApplicationService QuestionApplicationService { get; private set; }
        public ISectionApplicationService SectionApplicationService { get; private set; }

        public DynamicFormsApplicationServices(IMapper mapper,
            IFormApplicationService formApplicationService,
            IFormSubmissionApplicationService formSubmissionApplicationService,
            IFormSectionSubmissionApplicationService formSectionSubmissionApplicationService,
            ILookupTableApplicationService lookupTableApplicationService,
            IQuestionApplicationService questionApplicationService,
            ISectionApplicationService sectionApplicationService,
            IAuthorizationService authorizationService,
            IUserService userService)
            : base("forms.", mapper, authorizationService, userService)
        {
            FormApplicationService = formApplicationService;
            FormSubmissionApplicationService = formSubmissionApplicationService;
            FormSectionSubmissionApplicationService = formSectionSubmissionApplicationService;
            LookupTableApplicationService = lookupTableApplicationService;
            QuestionApplicationService = questionApplicationService;
            SectionApplicationService = sectionApplicationService;
        }

    }
}

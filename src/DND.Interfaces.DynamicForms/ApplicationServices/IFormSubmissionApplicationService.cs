﻿using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormSubmissionApplicationService : IBaseEntityApplicationService<FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto>
    {

    }
}
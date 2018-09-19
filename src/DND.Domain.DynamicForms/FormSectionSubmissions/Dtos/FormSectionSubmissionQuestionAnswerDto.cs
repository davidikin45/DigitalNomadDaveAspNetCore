using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSectionSubmissions.Dtos
{
    public class FormSectionSubmissionQuestionAnswerDto : DtoBase<int>, IMapFrom<FormSectionSubmissionQuestionAnswer>, IMapTo<FormSectionSubmissionQuestionAnswer>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [HiddenInput()]
        public int FormSectionSubmissionId { get; set; }

        public string FieldName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}

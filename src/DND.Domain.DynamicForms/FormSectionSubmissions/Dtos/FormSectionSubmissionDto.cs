using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSectionSubmissions.Dtos
{
    public class FormSectionSubmissionDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        public Guid FormSubmissionId { get; set; }

        public string UrlSlug { get; set; }

        public bool Valid { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Question and Answers")]
        [Repeater("{" + nameof(FormSectionSubmissionQuestionAnswerDto.Question) + "}")]
        public List<FormSectionSubmissionQuestionAnswerDto> QuestionAnswers { get; set; } = new List<FormSectionSubmissionQuestionAnswerDto>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FormSectionSubmission, FormSectionSubmissionDto>();

            configuration.CreateMap<FormSectionSubmissionDto, FormSectionSubmission>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}

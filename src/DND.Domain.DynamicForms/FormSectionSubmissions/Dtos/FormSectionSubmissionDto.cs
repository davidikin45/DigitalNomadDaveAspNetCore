using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSectionSubmissions.Dtos
{
    public class FormSectionSubmissionDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        public Guid FormSubmissionId { get; set; }

        public bool Completed { get; set; }
        public bool Valid { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Question and Answers")]
        [Repeater("{" + nameof(FormSectionSubmissionQuestionAnswerDto.Question) + "}")]
        public List<FormSectionSubmissionQuestionAnswerDto> QuestionAnswers { get; set; }

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

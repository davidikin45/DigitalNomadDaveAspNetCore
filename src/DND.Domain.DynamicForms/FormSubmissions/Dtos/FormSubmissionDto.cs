using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Domain.DynamicForms.FormSubmissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSubmissions.Dtos
{
    public class FormSubmissionDto : BaseDtoAggregateRoot<Guid>, IHaveCustomMappings
    {
        public bool Completed { get; set; }
        public bool Valid { get; set; }

        [Required]
        [Dropdown(typeof(Form), nameof(DND.Domain.DynamicForms.Forms.Form.Name))]
        public int FormId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public FormDto Form { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Sections")]
        [Repeater("{" + nameof(FormSectionSubmissionDto.QuestionAnswers) + "}")]
        public List<FormSectionSubmissionDto> Sections { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FormSubmission, FormSubmissionDto>();

            configuration.CreateMap<FormSubmissionDto, FormSubmission>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}

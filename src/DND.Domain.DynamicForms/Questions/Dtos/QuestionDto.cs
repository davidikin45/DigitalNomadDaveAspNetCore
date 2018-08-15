using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Domain.DynamicForms.Questions.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required]
        public string FieldName { get; set; }

        [Required()]
        public string QuestionText { get; set; }

        public QuestionType QuestionType { get; set; }

        [Dropdown(typeof(LookupTable), nameof(DND.Domain.DynamicForms.LookupTables.LookupTable.Name))]
        public Nullable<int> LookupTableId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public LookupTableDto LookupTable { get; set; }

        public string DefaultAnswer { get; set; }
        public string Placeholder { get; set; }
        public string HelpText { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Questions")]
        [Repeater("{" + nameof(QuestionQuestionDto.LogicQuestionId) + "}")]
        public List<QuestionQuestionDto> Questions { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Sections")]
        [Repeater("{" + nameof(QuestionSectionDto.SectionId) + "}")]
        public List<QuestionSectionDto> Sections { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Validations")]
        [Repeater("{" + nameof(QuestionValidationDto.ValidationType) + "}")]
        public List<QuestionValidationDto> Validations { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Question, QuestionDto>();

            configuration.CreateMap<QuestionDto, Question>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}

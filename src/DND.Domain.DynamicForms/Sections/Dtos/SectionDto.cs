using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Sections.Dtos
{
    public class SectionDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required()]
        public string Name { get; set; }

        public string UrlSlug { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Questions")]
        [Repeater("{" + nameof(SectionQuestionDto.QuestionId) + "}")]
        public List<SectionQuestionDto> Questions { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Child Sections")]
        [Repeater("{" + nameof(SectionSectionDto.ChildSectionId) + "}")]
        public List<SectionSectionDto> Sections { get; set; } = new List<SectionSectionDto>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Section, SectionDto>();

            configuration.CreateMap<SectionDto, Section>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}

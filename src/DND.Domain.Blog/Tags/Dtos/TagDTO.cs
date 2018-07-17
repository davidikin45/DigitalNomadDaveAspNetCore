using DND.Domain.Models;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Common.Implementation.Dtos;
using AutoMapper;
using DND.Domain.Blog.Locations.Dtos;

namespace DND.Domain.Blog.Tags.Dtos
{
    public class TagDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        public TagDto()
        {
            Locations = new List<LocationDto>()
            {
                new LocationDto()
                {
                    Id = 1,
                    Name = "Tag Name",
                    UrlSlug = "Url Slug"
                },
                new LocationDto()
                {
                     Id = 2,
                    Name = "Tag Name 2",
                    UrlSlug = "Url Slug 2"
                }
            };
        }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        [Render(ShowForCreate = false,ShowForEdit = false, ShowForGrid = false, ShowForDisplay = false)]
        public int Count { get; set; }

        [Render(ShowForGrid = true, LinkToCollectionInGrid = true, ShowForDisplay = false, ShowForEdit = true, ShowForCreate = true)]
        [Repeater(nameof(LocationDto.Name))]
        public List<LocationDto> Locations { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<TagDto, Tag>()
             .ForMember(bo => bo.DateModified, dto => dto.Ignore())
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore());

            configuration.CreateMap<Tag, TagDto>();
        }
    }
}

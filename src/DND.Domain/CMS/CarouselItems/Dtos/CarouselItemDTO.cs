using DND.Domain.Constants;
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

namespace DND.Domain.CMS.CarouselItems.Dtos
{
    public class CarouselItemDto : BaseDtoAggregateRoot<int>,IHaveCustomMappings
    {
        [Render(AllowSortForGrid = false)]
        [FolderDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        public string Title { get; set; }

        public string CarouselText
        { get; set; }

        [UIHint("String")]
        public string ButtonText { get; set; }
        public string Link { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileDropdown(Folders.Carousel, true)]
        public string File { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        [Required]
        public bool OpenInNewWindow
        { get; set; }

        [Required]
        public bool Published { get; set; }

        public CarouselItemDto()
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();


            if (string.IsNullOrEmpty(Album) && (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(ButtonText) || string.IsNullOrEmpty(Link) || string.IsNullOrEmpty(File)))
            {
                errors.Add(new ValidationResult("If an Album is not selected Title, Button Text, Link and File must be entered"));
            }

            if (!string.IsNullOrEmpty(Album) && !string.IsNullOrEmpty(File))
            {
                errors.Add(new ValidationResult("Please select an Album or File, but not both"));
            }

            return errors;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CarouselItemDto, CarouselItem>()
             .ForMember(bo => bo.DateModified, dto => dto.Ignore())
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore());

            configuration.CreateMap<CarouselItem, CarouselItemDto>();
        }
    }
}
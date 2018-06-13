using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Authors.Dtos
{
    public class AuthorDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        public AuthorDto()
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        { 
            configuration.CreateMap<AuthorDto, Author>()
             .ForMember(bo => bo.DateModified, dto => dto.Ignore())
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore());

            configuration.CreateMap<Author, AuthorDto>();
        }
    }
}
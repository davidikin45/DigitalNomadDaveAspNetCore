using AutoMapper;
using DND.Domain.Models;
using Solution.Base.Implementation.Models;
using Solution.Base.Interfaces.Automapper;
using Solution.Base.ModelMetadataCustom;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class MailingListDTO : BaseEntity<int>, IHaveCustomMappings
    {
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Render(ShowForEdit = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<MailingList, MailingListDTO>();

            configuration.CreateMap<MailingListDTO, MailingList>()
           .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}

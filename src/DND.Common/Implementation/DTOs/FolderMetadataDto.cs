using AutoMapper;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Dtos
{
    public class FolderMetadataDto : BaseEntity<string>, IHaveCustomMappings
    {
        [Render(ShowForDisplay = false, ShowForEdit = false, ShowForGrid = false)]
        public DirectoryInfo Folder { get; set; }

        [Render(AllowSortForGrid = true)]
        [Required]
        public DateTime CreationTime { get; set; }

        [Render(ShowForGrid = false), Required]
        public string Name { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<DirectoryInfo, FolderMetadataDto>()
            .ForMember(dto => dto.Id, bo => bo.MapFrom(s => s.FullName))
            .ForMember(dto => dto.Name, bo => bo.MapFrom(s => s.Name))
            .ForMember(dto => dto.CreationTime, bo => bo.MapFrom(s => s.LastWriteTime))
            .ForMember(dto => dto.Folder, bo => bo.MapFrom(s => s));
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}

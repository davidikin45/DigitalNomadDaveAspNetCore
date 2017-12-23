﻿using AutoMapper;
using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DND.Base.Implementation.DTOs
{
    public class FileMetadataDTO : BaseEntity<string>, IHaveCustomMappings
    {
        //[Render(ShowForDisplay = false, ShowForEdit = false, ShowForGrid = false)]
        public FileInfo File { get; set; }

        [Render(AllowSortForGrid = true)]
        [Required ]
        public DateTime CreationTime { get; set; }
        
        [Render(ShowForGrid = false), Required]
        public string Caption { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileInfo, FileMetadataDTO>()
            .ForMember(dto => dto.Id, bo => bo.MapFrom(s => s.FullName))
            .ForMember(dto => dto.Caption, bo => bo.MapFrom(s => Path.GetFileNameWithoutExtension(s.Name)))
            .ForMember(dto => dto.CreationTime, bo => bo.MapFrom(s => s.LastWriteTime))
            .ForMember(dto => dto.File, bo => bo.MapFrom(s => s));
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}

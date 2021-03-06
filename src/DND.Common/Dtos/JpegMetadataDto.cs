﻿using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Common.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;

namespace DND.Common.Dtos
{
    public class JpegMetadataDto : DtoBase<string>, IHaveCustomMappings
    {
        //[Render(ShowForDisplay =false, ShowForEdit = false, ShowForGrid = false)]
        public FileInfo Image { get; set; }

        [Required]
        public DateTime DateTaken { get; set; }

        [Required, Render(ShowForGrid = false)]
        public DateTime DateCreated { get; set; }

        [Render(ShowForGrid = false), Required]
        public string Caption { get; set; }

        public string PlaceId { get; set; }
        public DbGeography GPSLocation { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<JpegMetadata, JpegMetadataDto>()
            .ForMember(dto => dto.Id, bo => bo.MapFrom(s => s.Id))
            .ForMember(dto => dto.PlaceId, bo => bo.MapFrom(s => s.Comments))
            .ForMember(dto => dto.Caption, bo => bo.MapFrom(s => Path.GetFileNameWithoutExtension(s.FileInfo.Name)))
            .ForMember(dto => dto.DateTaken, bo => bo.MapFrom(s => s.DateTaken))
            .ForMember(dto => dto.DateCreated, bo => bo.MapFrom(s => s.FileInfo.LastWriteTime))
            .ForMember(dto => dto.Image, bo => bo.MapFrom(s => s.FileInfo))
            .ForMember(dto => dto.GPSLocation, bo => bo.MapFrom(s => s.Latitude.HasValue && s.Longitude.HasValue ?
            DbGeography.FromText(string.Format(CultureInfo.InvariantCulture, "POINT({1} {0})", s.Latitude.ToString(), s.Longitude.ToString()), DbGeography.DefaultCoordinateSystemId) : default(DbGeography)));

            configuration.CreateMap<JpegMetadataDto, JpegMetadata>()
           .ForMember(bo => bo.Comments, dto => dto.MapFrom(s => s.PlaceId))
           .ForMember(bo => bo.DateTaken, dto => dto.MapFrom(s => s.DateTaken))
           .ForMember(bo => bo.Latitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.Latitude : null))
           .ForMember(bo => bo.Longitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.Longitude : null));
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}

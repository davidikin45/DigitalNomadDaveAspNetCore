﻿using AutoMapper;
using DND.Domain.DTOs;
using DND.Common.Interfaces.Automapper;

namespace DND.Domain.Skyscanner.Model
{
    public class Locale : IHaveCustomMappings
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Locale, LocaleDTO>()
             .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Code))
             .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));
        }
    }
}

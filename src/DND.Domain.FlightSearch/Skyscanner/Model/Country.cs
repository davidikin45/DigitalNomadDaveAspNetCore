﻿using AutoMapper;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Domain.FlightSearch.Markets.Dtos;
using System.Collections.Generic;

namespace DND.Domain.Skyscanner.Model
{
    public partial class Country : IHaveCustomMappings
    {

        public string CurrencyId { get; set; }

        public List<Region> Regions { get; set; }

        public List<City> Cities { get; set; }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string LanguageId { get; set; }

        void IHaveCustomMappings.CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Country, MarketDto>()
               .ForMember(d => d.Id, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.Code) ? s.Id : s.Code))
               .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
               .ForMember(d => d.CurrencyId, opt => opt.MapFrom(s => s.CurrencyId))
               .ForMember(d => d.LanguageId, opt => opt.MapFrom(s => s.LanguageId));
        }
    }
}

using AutoMapper;
using DND.Domain.DTOs;
using Solution.Base.Interfaces.Automapper;
using System.Collections.Generic;

namespace DND.Services.Skyscanner.Model
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
            configuration.CreateMap<Country, MarketDTO>()
               .ForMember(d => d.Id, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.Code) ? s.Id : s.Code))
               .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
               .ForMember(d => d.CurrencyId, opt => opt.MapFrom(s => s.CurrencyId))
               .ForMember(d => d.LanguageId, opt => opt.MapFrom(s => s.LanguageId));
        }
    }
}

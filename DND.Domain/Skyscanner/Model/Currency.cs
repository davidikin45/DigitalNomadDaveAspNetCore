using AutoMapper;
using DND.Domain.DTOs;
using DND.Common.Interfaces.Automapper;

namespace DND.Domain.Skyscanner.Model
{
    public class Currency : IHaveCustomMappings
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public bool SymbolOnLeft { get; set; }
        public bool SpaceBetweenAmountAndSymbol { get; set; }
        public int RoundingCoefficient { get; set; }
        public int DecimalDigits { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Currency, CurrencyDTO>()
             .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Code))
             .ForMember(d => d.Symbol, opt => opt.MapFrom(s => s.Symbol))
             .ForMember(d => d.ThousandsSeparator, opt => opt.MapFrom(s => s.ThousandsSeparator))
             .ForMember(d => d.DecimalSeparator, opt => opt.MapFrom(s => s.DecimalSeparator))
             .ForMember(d => d.SymbolOnLeft, opt => opt.MapFrom(s => s.SymbolOnLeft))
             .ForMember(d => d.SpaceBetweenAmountAndSymbol, opt => opt.MapFrom(s => s.SpaceBetweenAmountAndSymbol))
             .ForMember(d => d.RoundingCoefficient, opt => opt.MapFrom(s => s.RoundingCoefficient))
             .ForMember(d => d.DecimalDigits, opt => opt.MapFrom(s => s.DecimalDigits));
        }
    }
}

using DND.Domain.FlightSearch.Currencies.Dtos;

namespace DND.Domain.FlightSearch.Markets.Dtos
{
    public class MarketDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LanguageId { get; set; }
        public string CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }
    }
}

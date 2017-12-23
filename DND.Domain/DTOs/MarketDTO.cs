namespace DND.Domain.DTOs
{
    public class MarketDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LanguageId { get; set; }
        public string CurrencyId { get; set; }
        public CurrencyDTO Currency { get; set; }
    }
}

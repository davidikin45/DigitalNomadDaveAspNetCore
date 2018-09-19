namespace DND.Common.Infrastrucutre.Interfaces.Domain.Dtos
{
    public interface IDtoConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}

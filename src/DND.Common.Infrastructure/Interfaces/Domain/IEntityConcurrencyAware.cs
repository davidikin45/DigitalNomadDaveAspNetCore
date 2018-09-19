namespace DND.Common.Infrastrucutre.Interfaces.Domain
{
    public interface IEntityConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}

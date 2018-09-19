using DND.Domain.DynamicForms.LookupTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.LookupTables
{
    public class LookupTableItemConfiguration
           : IEntityTypeConfiguration<LookupTableItem>
    {

        public void Configure(EntityTypeBuilder<LookupTableItem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);
        }
    }
}

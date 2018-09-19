using DND.Domain.DynamicForms.LookupTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.LookupTables
{
    public class LookupTableConfiguration
           : IEntityTypeConfiguration<LookupTable>
    {

        public void Configure(EntityTypeBuilder<LookupTable> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}

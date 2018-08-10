using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.LookupTables
{
    public class LookupTableConfiguration
           : EntityTypeConfiguration<LookupTable>
    {
        public LookupTableConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();
                 
        }
    }
}

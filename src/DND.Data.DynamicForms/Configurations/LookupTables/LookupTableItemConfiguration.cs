using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.LookupTables
{
    public class LookupTableItemConfiguration
           : EntityTypeConfiguration<LookupTableItem>
    {
        public LookupTableItemConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);
        }
    }
}

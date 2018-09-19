using DND.Domain.DynamicForms.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Forms
{
    public class FormConfiguration
           : IEntityTypeConfiguration<Form>
    {

        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}

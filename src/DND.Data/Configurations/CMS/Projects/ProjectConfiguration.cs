using DND.Domain.Blog.Tags;
using DND.Domain.CMS.Projects;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.CMS.Projects
{
    public class ProjectConfiguration
           : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.Name)
                 .IsRequired()
                .HasMaxLength(100);

            Property(p => p.DescriptionText)
                .IsRequired()
               .HasMaxLength(200);

        }
    }
}

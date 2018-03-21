using DND.Domain.Blog.Tags;
using DND.Domain.CMS.Projects;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.Projects
{
    public class ProjectConfiguration
           : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.Name)
                 .IsRequired()
                .HasMaxLength(100);

            Property(p => p.DescriptionText)
                .IsRequired()
               .HasMaxLength(200);

        }
    }
}

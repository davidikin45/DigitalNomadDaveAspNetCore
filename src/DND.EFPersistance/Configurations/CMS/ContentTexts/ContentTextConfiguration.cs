using DND.Domain.Blog.Tags;
using DND.Domain.CMS.ContentTexts;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.ContentTexts
{
    public class ContentTextConfiguration
           : EntityTypeConfiguration<ContentText>
    {
        public ContentTextConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.RowVersion).IsRowVersion();
        }
    }
}

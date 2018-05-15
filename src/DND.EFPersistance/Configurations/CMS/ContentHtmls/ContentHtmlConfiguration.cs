using DND.Domain.CMS.ContentHtmls;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.ContentHtmls
{
    public class ContentHtmlConfiguration
           : EntityTypeConfiguration<ContentHtml>
    {
        public ContentHtmlConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.RowVersion).IsRowVersion();

        }
    }
}

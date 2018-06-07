using DND.Domain.Blog.Tags;
using DND.Domain.CMS.Faqs;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.CMS.Faqs
{
    public class FaqConfiguration
           : EntityTypeConfiguration<Faq>
    {
        public FaqConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.Question)
                 .IsRequired();

            Property(p => p.Answer)
                .IsRequired();
        }
    }
}

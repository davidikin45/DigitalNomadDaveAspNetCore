using DND.Domain.CMS.MailingLists;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.CMS.MailingLists
{
    public class MailingListConfiguration
           : EntityTypeConfiguration<MailingList>
    {
        public MailingListConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            Property(p => p.Email)
                 .IsRequired();
        }
    }
}

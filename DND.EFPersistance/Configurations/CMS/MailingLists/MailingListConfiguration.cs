using DND.Domain.CMS.MailingLists;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.MailingLists
{
    public class MailingListConfiguration
           : EntityTypeConfiguration<MailingList>
    {
        public MailingListConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.Email)
                 .IsRequired();
        }
    }
}

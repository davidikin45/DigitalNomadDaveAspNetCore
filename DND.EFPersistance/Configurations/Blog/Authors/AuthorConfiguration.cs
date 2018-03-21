using DND.Domain.Blog.Authors;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.EFPersistance.Configurations.Blog.Authors
{
    public class AuthorConfiguration
           : EntityTypeConfiguration<Author>
    {
        public AuthorConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.Name)
                .IsRequired();

            Property(p => p.UrlSlug)
               .HasMaxLength(50);
        }
    }
}

using DND.Domain.Blog.Tags;
using DND.Domain.CMS.Testimonials;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.Configurations.CMS.Testimonials
{
    public class TestimonialConfiguration
           : EntityTypeConfiguration<Testimonial>
    {
        public TestimonialConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion(); // Only works for byte array. Will create a timestamp in EF6, rowversion EFCore
            //Property(p => p.RowVersion).IsConcurrencyToken().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed); //works for int

            Property(p => p.Source)
                 .IsRequired()
                .HasMaxLength(100);

            Property(p => p.QuoteText)
                .IsRequired()
               .HasMaxLength(5000);
        }
    }
}

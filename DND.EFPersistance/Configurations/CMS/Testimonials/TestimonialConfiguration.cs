using DND.Domain.Blog.Tags;
using DND.Domain.CMS.Testimonials;
using System.Data.Entity.ModelConfiguration;

namespace DND.EFPersistance.Configurations.CMS.Testimonials
{
    public class TestimonialConfiguration
           : EntityTypeConfiguration<Testimonial>
    {
        public TestimonialConfiguration()
        {
            HasKey(p => p.Id);

            Property(p => p.RowVersion).IsRowVersion(); // Only works for byte array.
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

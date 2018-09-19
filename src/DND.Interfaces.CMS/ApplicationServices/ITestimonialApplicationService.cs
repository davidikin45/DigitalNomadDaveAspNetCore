using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.Testimonials.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface ITestimonialApplicationService : IApplicationServiceEntity<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto>
    {
        
    }
}

using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.Testimonials.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface ITestimonialApplicationService : IBaseEntityApplicationService<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto>
    {
        
    }
}

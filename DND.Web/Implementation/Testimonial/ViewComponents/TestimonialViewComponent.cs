using DND.Common.Helpers;
using DND.Common.Interfaces.Repository;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ViewComponents;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Implementation.Testimonial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Implementation.Testimonial.ViewComponents
{
    public class TestimonialViewComponent : BaseViewComponent
    {
        private readonly ITestimonialApplicationService _testimonialService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepository;

        public TestimonialViewComponent(ITestimonialApplicationService testimonialService, IFileSystemRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _testimonialService = testimonialService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(TestimonialDto.DateCreated);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<TestimonialDto> testimonials = null;

            var testimonialsTask = _testimonialService.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<TestimonialDto>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, testimonialsTask);

            testimonials = testimonialsTask.Result;


            var viewModel = new TestimonialsViewModel
            {
                Testimonials = testimonials.ToList()
            };

            return View(viewModel);
        }

    }
}

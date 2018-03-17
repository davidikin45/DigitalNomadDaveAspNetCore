using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Models.TestimonialViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using Solution.Base.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
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
            string orderColumn = nameof(TestimonialDTO.DateCreated);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<TestimonialDTO> testimonials = null;

            var testimonialsTask = _testimonialService.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<TestimonialDTO>(orderColumn, orderType), null, null);

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

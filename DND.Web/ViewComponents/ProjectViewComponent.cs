using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Models.ProjectsViewModels;
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
    public class ProjectViewComponent : BaseViewComponent
    {
        private readonly IProjectApplicationService _projectService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepository;

        public ProjectViewComponent(IProjectApplicationService projectService, IFileSystemRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(ProjectDTO.DateCreated);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<ProjectDTO> projects = null;


            var projectsTask = _projectService.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<ProjectDTO>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, projectsTask);

            projects = projectsTask.Result;


            var viewModel = new ProjectsViewModel
            {
                Projects = projects.ToList()
            };

            return View(viewModel);
        }

    }
}

using DND.Common.Helpers;
using DND.Common.Interfaces.Repository;
using DND.Common.ViewComponents;
using DND.Domain.CMS.Projects.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Web.Mvc.Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Mvc.Project.ViewComponents
{
    public class ProjectViewComponent : BaseViewComponent
    {
        private readonly IProjectApplicationService _projectService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemRepository;

        public ProjectViewComponent(IProjectApplicationService projectService, IFileSystemGenericRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(ProjectDto.DateCreated);
            string orderType = "desc";

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<ProjectDto> projects = null;


            var projectsTask = _projectService.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<ProjectDto>(orderColumn, orderType), null, null);

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

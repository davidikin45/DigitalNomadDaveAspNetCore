using DND.Domain.CMS.Projects.Dtos;
using System.Collections.Generic;

namespace DND.Web.Mvc.Project.Models
{
    public class ProjectsViewModel
    {
        public IList<ProjectDto> Projects { get; set; }
    }
}

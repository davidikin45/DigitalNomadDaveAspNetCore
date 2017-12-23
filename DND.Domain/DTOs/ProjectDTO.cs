using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using DND.Domain.Constants;
using DND.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class ProjectDTO : BaseEntity<int>, IMapFrom<Project>, IMapTo<Project>
    {

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Link { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileDropdown(Folders.Projects, true)]
        public string File { get; set; }

        [Render(AllowSortForGrid = false)]
        [FolderDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        [Required, StringLength(200)]
        public string DescriptionText { get; set; }


        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}

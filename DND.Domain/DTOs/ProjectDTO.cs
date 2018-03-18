﻿using DND.Domain.Constants;
using DND.Domain.Models;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
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

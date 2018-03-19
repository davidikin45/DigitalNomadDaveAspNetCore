using DND.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Models.SidebarViewModels
{
    public class BlogWidgetViewModel
    {
        public IList<CategoryDTO> Categories { get; set; }
        public IList<TagDTO> Tags { get; set; }
        public IList<BlogPostDTO> LatestPosts { get; set; }
        public IList<FileInfo> LatestPhotos { get; set; }
    }
}

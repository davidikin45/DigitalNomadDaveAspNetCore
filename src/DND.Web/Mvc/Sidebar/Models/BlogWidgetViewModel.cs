using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Tags.Dtos;
using System.Collections.Generic;
using System.IO;

namespace DND.Web.Mvc.Sidebar.Models
{
    public class BlogWidgetViewModel
    {
        public IList<CategoryDto> Categories { get; set; }
        public IList<TagDto> Tags { get; set; }
        public IList<BlogPostDto> LatestPosts { get; set; }
        public IList<FileInfo> LatestPhotos { get; set; }
    }
}

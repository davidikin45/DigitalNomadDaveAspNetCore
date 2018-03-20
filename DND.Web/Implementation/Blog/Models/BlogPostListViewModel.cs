using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Tags.Dtos;
using System.Collections.Generic;

namespace DND.Web.Implementation.Blog.Models
{
    public class BlogPostListViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<BlogPostDto> Posts { get; set; }
        public int TotalPosts { get; set; }
        public CategoryDto Category { get; set; }
        public TagDto Tag { get; set; }
        public AuthorDto Author { get; set; }
        public string Search { get; set; }
    }
}

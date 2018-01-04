using DND.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Models.BlogPostViewModels
{
    public class BlogPostListViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<BlogPostDTO> Posts { get; set; }
        public int TotalPosts { get; set; }
        public CategoryDTO Category { get; set; }
        public TagDTO Tag { get; set; }
        public AuthorDTO Author { get; set; }
        public string Search { get; set; }
    }
}

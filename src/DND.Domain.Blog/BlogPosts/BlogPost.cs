using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPost : EntityAggregateRootAuditableBase<int>
    {

        //[Required, StringLength(500)]
        public string Title
        { get; set; }

        //[Required, StringLength(5000)]
        public string ShortDescription
        { get; set; }

        //[Required, StringLength(30000)]
        public string Description
        { get; set; }

        //[Required, StringLength(200)]
        public string UrlSlug
        { get; set; }

        public string CarouselImage
        { get; set; }

        //[StringLength(200)]
        public string CarouselText
        { get; set; }

        //[Required]
        public bool ShowInCarousel
        { get; set; }

        //[Required]
        public bool Published
        { get; set; }

        //[Required]
        public override DateTime DateCreated
        {
            get { return base.DateCreated; }
            set { base.DateCreated = value; }
        }

        //[Required]
        public int AuthorId { get; set; }
        public virtual Author Author
        { get; set; }

        //[Required]
        public int CategoryId { get; set; }
        public virtual Category Category
        { get; set; }

        public virtual IList<BlogPostTag> Tags
        { get; set; }

        public virtual IList<BlogPostLocation> Locations
        { get; set; }

        //[Required]
        public bool ShowLocationDetail { get; set; }

        //[Required]
        public bool ShowLocationMap { get; set; }

        //[Required]
        public int MapHeight { get; set; }

        //[Required]
        public int MapZoom { get; set; }

        //[Required]
        public string Album { get; set; }

        //[Required]
        public bool ShowPhotosInAlbum { get; set; }

        public BlogPost()
		{
            Tags = new List<BlogPostTag>();
            Locations = new List<BlogPostLocation>();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}
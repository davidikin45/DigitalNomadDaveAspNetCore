using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Blog.Tags;
using DND.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required(ErrorMessage = "Title: Field is required")]
        [StringLength(500, ErrorMessage = "Title: Length should not exceed 500 characters")]
        public string Title { get; set; }

        [Render(ShowForGrid = false)]
        [Required(ErrorMessage = "Short Description: Field is required")]
        [StringLength(5000, ErrorMessage = " Short Description: length should not exceed 5000 characters")]
        [MultilineText(HTML = false, Rows = 10)]
        public string ShortDescription { get; set; }

        [Render(ShowForGrid = false)]
        [Required(ErrorMessage = "Description: Field is required")]
        [StringLength(30000, ErrorMessage = "Description: length should not exceed 30000 characters")]
        [MultilineText(HTML = true, Rows = 40)]
        public string Description { get; set; }

        [Required]
        [Dropdown(typeof(Author), nameof(DND.Domain.Blog.Authors.Author.Name))]
        public int AuthorId { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public AuthorDto Author { get; set; }

        [Required]
        [Dropdown(typeof(Category), nameof(DND.Domain.Blog.Categories.Category.Name))]
        public int CategoryId { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public CategoryDto Category { get; set; }

        [Render(AllowSortForGrid = false)]
        [Dropdown(typeof(Tag), nameof(Tag.Name))]
        public List<int> TagIds { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public List<BlogPostTagDto> Tags { get; set; }

        [Render(ShowForGrid = false, AllowSortForGrid = false)]
        [Dropdown(typeof(Location), "{" + nameof(Location.LocationTypeString) + "} - {" + nameof(Location.Name) + "}", nameof(Location.Id), OrderByType.Descending)]
        public List<int> LocationIds { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public List<LocationDto> Locations { get; set; }

        [Required]
        public bool ShowLocationDetail { get; set; }

        [Required]
        public bool ShowLocationMap { get; set; }

        [Required]
        public int MapHeight { get; set; }

        [Required]
        public int MapZoom { get; set; }

        [Render(AllowSortForGrid = false)]
        [Required]
        [FolderDropdown(Folders.Gallery)]
        public string Album { get; set; }

        [Required]
        public bool ShowPhotosInAlbum { get; set; }

        [Render(ShowForGrid = false)]
        [FileDropdown(Folders.Gallery, true)]
        public string CarouselImage
        { get; set; }

        [Render(ShowForGrid = false)]
        [StringLength(200)]
        public string CarouselText
        { get; set; }

        [Required]
        public bool ShowInCarousel
        { get; set; }

        [StringLength(200, ErrorMessage = "UrlSlug: length should not exceed 200 characters")]
        public string UrlSlug { get; set; }

        [Required]
        public bool Published { get; set; }

        [Render(ShowForEdit = false)]
        public DateTime? DateModified { get; set; }

        [Render(ShowForEdit = false)]
        public DateTime DateCreated { get; set; }

        public BlogPostDto()
        {
            MapHeight = 300;
            MapZoom = 7;
            TagIds = new List<int>();
            Tags = new List<BlogPostTagDto>();
            LocationIds = new List<int>();
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<BlogPost, BlogPostDto>()
              .ForMember(dto => dto.Tags, bo => bo.MapFrom(s => s.Tags.Select(y => y.Tag).ToList()))
              .ForMember(dto => dto.TagIds, bo => bo.MapFrom(s => s.Tags.Select(y => y.Tag.Id).ToList()))
              .ForMember(dto => dto.Locations, bo => bo.MapFrom(s => s.Locations.Select(y => y.Location).ToList()))
               .ForMember(dto => dto.LocationIds, bo => bo.MapFrom(s => s.Locations.Select(y => y.Location.Id).ToList()));

            configuration.CreateMap<BlogPostDto, BlogPost>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore())
                 .ForMember(bo => bo.Tags, dto => dto.ResolveUsing(new BlogPostTagResolver()))
                .ForMember(bo => bo.Locations, dto => dto.ResolveUsing(new BlogPostLocationResolver()));
        }

        public class BlogPostLocationResolver : IValueResolver<BlogPostDto, BlogPost, IList<BlogPostLocation>>
        {
            public IList<BlogPostLocation> Resolve(BlogPostDto source, BlogPost destination, IList<BlogPostLocation> destMember, ResolutionContext context)
            {
                var updatedLocations = new List<BlogPostLocation>();
                foreach (var locationId in source.LocationIds)
                {
                    var location = new BlogPostLocation();
                    location.LocationId = locationId;
                    location.BlogPostId = destination.Id;

                    var existingTag = destination.Locations.SingleOrDefault(l => l.LocationId == locationId);

                    // No existing with this id, so add a new one
                    if (existingTag == null)
                    {
                        updatedLocations.Add(context.Mapper.Map<BlogPostLocation>(location));
                    }
                    // Existing found, so map to existing instance
                    else
                    {
                        context.Mapper.Map(location, existingTag);
                        updatedLocations.Add(existingTag);
                    }
                }
                return updatedLocations;
            }
        }

        public class BlogPostTagResolver : IValueResolver<BlogPostDto, BlogPost, IList<BlogPostTag>>
        {
            public IList<BlogPostTag> Resolve(BlogPostDto source, BlogPost destination, IList<BlogPostTag> destMember, ResolutionContext context)
            {
                var updatedTags = new List<BlogPostTag>();
                foreach (var tagId in source.TagIds)
                {
                    var tag = new BlogPostTag();
                    tag.TagId = tagId;
                    tag.BlogPostId = destination.Id;

                    var existingTag = destination.Tags.SingleOrDefault(t => t.TagId == tagId);

                    // No existing with this id, so add a new one
                    if (existingTag == null)
                    {
                        updatedTags.Add(context.Mapper.Map<BlogPostTag>(tag));
                    }
                    // Existing found, so map to existing instance
                    else
                    {
                        context.Mapper.Map(tag, existingTag);
                        updatedTags.Add(existingTag);
                    }
                }
                return updatedTags;
            }
        }
    }
}

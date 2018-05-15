using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Blog.Tags;
using DND.Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostDeleteDto : BaseDtoAggregateRoot<int>,  IMapFrom<BlogPost>, IMapTo<BlogPost>
    {

        public BlogPostDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}

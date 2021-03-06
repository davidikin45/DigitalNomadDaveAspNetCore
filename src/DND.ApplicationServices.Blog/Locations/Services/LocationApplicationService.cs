﻿using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Users;
using DND.Common.Infrastructure.Validation;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Locations.Services
{
    public class LocationApplicationService : ApplicationServiceEntityBase<Location, LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationDomainService>, ILocationApplicationService
    {
        public LocationApplicationService(ILocationDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<LocationDto>> hubContext)
        : base("blog.locations.", domainService, mapper, authorizationService, userService)
        {

        }

        public override Task<Result<LocationDto>> CreateAsync(LocationDto dto, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.CreateAsync(dto, createdBy, cancellationToken);
        }

        public async Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug));
            return Mapper.Map<LocationDto>(bo);
        }

        public override Task<Result> UpdateAsync(object id,  LocationDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.UpdateAsync(id, dto, updatedBy, cancellationToken);
        }


    }
}
﻿using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.Testimonials;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    public class TestimonialApplicationService : BaseEntityApplicationService<Testimonial, TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialDomainService>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<TestimonialDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
     
    }
}
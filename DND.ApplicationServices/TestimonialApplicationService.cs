﻿using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;

namespace DND.ApplicationServices
{
    public class TestimonialApplicationService : BaseEntityApplicationService<IApplicationDbContext, Testimonial, TestimonialDTO, ITestimonialDomainService>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}
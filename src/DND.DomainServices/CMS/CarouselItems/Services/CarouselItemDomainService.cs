using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.CarouselItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using DND.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace DND.DomainServices.CMS.CarouselItems.Services
{
    public class CarouselItemDomainService : BaseEntityDomainService<IApplicationDbContext, CarouselItem>, ICarouselItemDomainService
    {
        public CarouselItemDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(CarouselItem entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}
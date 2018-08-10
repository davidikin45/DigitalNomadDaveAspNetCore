using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.LookupTables.Dtos;

namespace DND.Interfaces.DynamicForms.DomainServices
{
    public interface ILookupTableApplicationService : IBaseEntityApplicationService<LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto>
    {

    }
}

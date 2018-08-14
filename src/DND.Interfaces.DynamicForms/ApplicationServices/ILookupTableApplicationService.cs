using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.LookupTables.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface ILookupTableApplicationService : IBaseEntityApplicationService<LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto>
    {

    }
}

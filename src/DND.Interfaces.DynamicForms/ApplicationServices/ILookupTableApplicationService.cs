using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.LookupTables.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface ILookupTableApplicationService : IApplicationServiceEntity<LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto>
    {

    }
}

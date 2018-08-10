using DND.Common.Interfaces.DomainServices;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.DynamicForms.DomainServices
{
    public interface IFormDomainService : IBaseEntityDomainService<Form>
    {
        Task<Form> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken);
    }
}

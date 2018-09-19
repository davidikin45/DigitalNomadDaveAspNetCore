using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastrucutre.Interfaces.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DND.Common.Domain.Dtos
{
    public abstract class DtoAggregateRootBase<T> : DtoBase<T>, IDtoConcurrencyAware
    {
        //Optimistic Concurrency
        [HiddenInput, Render(ShowForCreate = false, ShowForDisplay = false, ShowForEdit = true, ShowForGrid = false)]
        public virtual byte[] RowVersion { get; set; }
    }
}

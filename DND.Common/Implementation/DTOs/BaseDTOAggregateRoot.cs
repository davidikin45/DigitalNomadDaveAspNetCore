using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Dtos;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Dtos
{

    public abstract class BaseDtoAggregateRoot<T> : BaseDto<T>, IBaseDtoConcurrencyAware
    {
        //Optimistic Concurrency
        [HiddenInput, Render(ShowForCreate = false, ShowForDisplay = false, ShowForEdit = true, ShowForGrid = false)]
        public byte[] RowVersion { get; set; }
    }
}

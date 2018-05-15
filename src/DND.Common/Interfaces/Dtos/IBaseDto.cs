using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Dtos
{
    public interface IBaseDto : IBaseObjectValidatable
    {

    }

    public interface IBaseDtoWithId : IBaseDto
    {
        object Id { get; set; }
    }

    public interface IBaseDto<T> : IBaseDtoWithId
    {
         new T Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Models
{
    public interface IBaseEntityAuditable : IBaseEntity
    {
        DateTime DateCreated { get; set; }
        string UserCreated { get; set; }
        DateTime? DateModified { get; set; }
        string UserModified { get; set; }
    }
}

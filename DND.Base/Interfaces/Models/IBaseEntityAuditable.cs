using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.Interfaces.Model
{
    public interface IBaseEntityAuditable : IBaseEntity
    {
        DateTime DateCreated { get; set; }
        string UserCreated { get; set; }
        DateTime? DateModified { get; set; }
        string UserModified { get; set; }
    }
}
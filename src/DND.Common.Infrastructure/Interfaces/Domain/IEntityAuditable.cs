using System;

namespace DND.Common.Infrastrucutre.Interfaces.Domain
{
    public interface IEntityAuditable
    {
        DateTime DateCreated { get; set; }
        string UserCreated { get; set; }
        DateTime? DateModified { get; set; }
        string UserModified { get; set; }
        DateTime? DateDeleted { get; set; }
        string UserDeleted { get; set; }

        string UserOwner { get; set; }
    }
}

using DND.Base.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.Implementation.Models
{
    public abstract class BaseEntityAuditable<T> : BaseEntity<T>, IBaseEntityAuditable
    {
        private DateTime? _dateCreated;
        public virtual DateTime DateCreated
        {
            get { return _dateCreated ?? DateTime.UtcNow; }
            set { _dateCreated = value; }
        }

        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }
    }
}

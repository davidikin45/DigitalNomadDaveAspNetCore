using DND.Common.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseEntityAuditable<T> : BaseEntity<T>, IBaseEntityAuditable where T : IEquatable<T>
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

        //[NotMapped]
        public DateTime? DateDeleted { get; set; }
        //[NotMapped]
        public string UserDeleted { get; set; }
    }
}

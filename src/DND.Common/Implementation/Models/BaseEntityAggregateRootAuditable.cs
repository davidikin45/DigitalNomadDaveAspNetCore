﻿using DND.Common.Interfaces.Models;
using System;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseEntityAggregateRootAuditable<T> : BaseEntityAggregateRoot<T>, IBaseEntityAuditable where T : IEquatable<T>
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

        public DateTime? DateDeleted { get; set; }
        public string UserDeleted { get; set; }
    }
}

using DND.Common.Interfaces.Models;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseEntity<T> : BaseObjectValidatable, IBaseEntity<T> where T : IEquatable<T>
    {
        [ReadOnlyHiddenInput(ShowForCreate = true, ShowForEdit = true), Display(Order = 0)]
        public virtual T Id { get; set; }

        [ReadOnlyHiddenInput(ShowForCreate = true, ShowForEdit = true), Display(Order = 0)]
        object IBaseEntity.Id
        {
            get { return this.Id; }
            set { this.Id = (T)value;  }
        }

        //[ReadOnly(true), Display(Order = 0), HiddenInput()]
        //public virtual T Id { get; set; }

        //[ReadOnly(true), Display(Order = 0), HiddenInput()]
        //object IBaseEntity.Id
        //{
        //    get { return this.Id; }
        //    set { this.Id = (T)value; }
        //}

        public override bool Equals(object obj)
        {
            var other = obj as BaseEntity<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if(Id.ToString() == "0" || other.Id.ToString() == "0" )
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(BaseEntity<T> a, BaseEntity<T> b)
        {
           if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity<T> a, BaseEntity<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id.ToString()).GetHashCode();
        }
    }
}

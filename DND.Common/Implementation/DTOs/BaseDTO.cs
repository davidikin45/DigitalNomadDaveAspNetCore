using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Dtos;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Dtos
{
    public abstract class BaseDto : BaseObjectValidatable, IBaseDto
    {
        
    }

    public abstract class BaseDto<T> : BaseDto, IBaseDto<T>
    {
        [ReadOnlyHiddenInput(ShowForCreate = true, ShowForEdit = true), Display(Order = 0)]
        public virtual T Id { get; set; }

        [ReadOnlyHiddenInput(ShowForCreate = true, ShowForEdit = true), Display(Order = 0)]
        object IBaseDtoWithId.Id
        {
            get { return this.Id; }
            set { this.Id = (T)value; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as BaseEntity<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id.ToString() == "0" || other.Id.ToString() == "0")
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(BaseDto<T> a, BaseDto<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseDto<T> a, BaseDto<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id.ToString()).GetHashCode();
        }
    }
}

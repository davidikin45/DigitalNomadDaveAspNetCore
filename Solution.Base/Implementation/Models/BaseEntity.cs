using Solution.Base.Interfaces.Models;
using Solution.Base.ModelMetadataCustom;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System.ComponentModel.DataAnnotations;

namespace Solution.Base.Implementation.Models
{
    public abstract class BaseEntity<T> : BaseObjectValidatable, IBaseEntity<T>
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

    }
}

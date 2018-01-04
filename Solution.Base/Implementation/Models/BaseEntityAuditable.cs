using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solution.Base.Interfaces.Models;
using System.ComponentModel.DataAnnotations;
using Solution.Base.Interfaces.Models;

namespace Solution.Base.Implementation.Models
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

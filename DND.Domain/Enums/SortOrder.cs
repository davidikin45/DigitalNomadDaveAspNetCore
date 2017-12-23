using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Enums
{
    public enum SortOrder
    {
        [Display(Name = "Ascending"), Description("Asc")]
        Asc,
        [Display(Name = "Descending"), Description("Desc")]
        Desc
    }
}

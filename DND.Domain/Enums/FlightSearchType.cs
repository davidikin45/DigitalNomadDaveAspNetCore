using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Enums
{
    public enum FlightSearchType
    {
        [Description("Return")]
        [Display(Name = "Return")]
        Return,
        [Description("One way")]
        [Display(Name = "One way")]
        OneWay
    }
}

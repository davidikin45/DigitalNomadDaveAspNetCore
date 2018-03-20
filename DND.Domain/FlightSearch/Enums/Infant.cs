using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.FlightSearch.Enums
{
    public enum Infant
    {
        [Description("0")]
        [Display(Name = "0")]
        Zero = 0,
        [Description("1")]
        [Display(Name = "1")]
        One = 1,
        [Description("2")]
        [Display(Name = "2")]
        Two = 2,
        [Description("3")]
        [Display(Name = "3")]
        Three = 3,
        [Description("4")]
        [Display(Name = "4")]
        Four = 4,
        [Description("5")]
        [Display(Name = "5")]
        Five = 5,
        [Description("6")]
        [Display(Name = "6")]
        Six = 6,
        [Description("7")]
        [Display(Name = "7")]
        Seven = 7,
        [Description("8")]
        [Display(Name = "8")]
        Eight = 8
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.FlightSearch.Enums
{
    public enum FlightClass
    {
        [Display(Name = "Economy"), Description("Economy")]
        Economy,
        [Display(Name = "Premium Economy"), Description("PremiumEconomy")]
        PremiumEconomy,
        [Display(Name = "Business Class"), Description("Business")]
        Business,
        [Display(Name = "First Class"), Description("First")]
        First
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.FlightSearch.Enums
{
    public enum LocationType
    {
        [Display(Name = "Cafe")]
        Cafe,
        [Display(Name = "Restaurant")]
        Restaurant,
        [Display(Name = "Tourist Attraction")]
        TouristAttraction,
        [Display(Name = "Beach")]
        Beach,
        [Display(Name = "Bar")]
        Bar,
        [Display(Name = "Transport")]
        Transport,
        [Display(Name = "Accomodation")]
        Accomodation,
        [Display(Name = "Airport")]
        Airport,
        [Display(Name = "Region")]
        Region,
        [Display(Name = "City")]
        City,
        [Display(Name = "Country")]
        Country,
        [Display(Name = "Other")]
        Other
    }
}

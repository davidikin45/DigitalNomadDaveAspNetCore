using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Skyscanner.Model
{
    public partial class Airport
    {

        public string CityId { get; set; }

        public string CountryId { get; set; }

        public string Location { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string RegionId { get; set; }
    }
}

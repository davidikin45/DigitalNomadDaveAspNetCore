using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class Place
    {

        public string PlaceId { get; set; }

        public string IataCode { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string SkyscannerCode { get; set; }

        public string CityName { get; set; }

        public string CityId { get; set; }

        public string CountryName { get; set; }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Code { get; set; }

        public string PlaceName { get; set; }
        public string CountryId { get; set; }
        public string RegionId { get; set; }
 

    }
}

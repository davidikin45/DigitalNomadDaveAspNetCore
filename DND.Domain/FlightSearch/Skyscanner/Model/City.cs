﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Skyscanner.Model
{
    public partial class City
    {

        public bool SingleAirportCity { get; set; }

        public List<Airport> Airports { get; set; }

        public string CountryId { get; set; }

        public string Location { get; set; }

        public string IataCode { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string RegionId { get; set; }
    }

}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Skyscanner.Model
{
    public partial class Airport
    {

        public double Latitude
        {
            get
            {
                if (!string.IsNullOrEmpty(Location))
                {
                    var split = Location.Split(',');
                    return double.Parse(split[0].Trim());
                }
                return 0;
            }
        }

        public double Longitude
        {
            get
            {
                if (!string.IsNullOrEmpty(Location))
                {
                    var split = Location.Split(',');
                    return double.Parse(split[1].Trim());
                }
                return 0;
            }
        }
    }
}

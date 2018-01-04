using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class PricingOption
    {

        public int PriceRounded { get; set; }

        public string PriceFormatted { get; set; }
        public string PriceRoundedFormatted { get; set; }
    }
}

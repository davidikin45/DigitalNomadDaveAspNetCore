using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class PricingOption
    {
        public List<int> Agents { get; set; }
        public int QuoteAgeInMinutes { get; set; }
        public double Price { get; set; }
        public string DeeplinkUrl { get; set; }
    }
}

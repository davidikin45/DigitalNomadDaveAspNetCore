using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public class BrowseGridServiceResponse
    {

        public string[][] Dates { get; set; }

        public List<Place> Places { get; set; }

        public List<Carrier> Carriers { get; set; }

        public List<Currency> Currencies { get; set; }
    }
}

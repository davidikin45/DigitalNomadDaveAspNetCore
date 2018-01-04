using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//http://json2csharp.com/
namespace DND.Services.Skyscanner.Model
{
    public partial class BrowseDatesServiceResponse
        {
            public List<Quote> Quotes { get; set; }
            public List<Place> Places { get; set; }
            public List<Carrier> Carriers { get; set; }
            public List<Currency> Currencies { get; set; }
        }
}

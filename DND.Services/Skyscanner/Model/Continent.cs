using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public class Continent
    {

        public List<Country> Countries { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

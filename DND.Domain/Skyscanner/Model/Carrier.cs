using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Skyscanner.Model
{
    public class Carrier
    {
        public int CarrierId { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string DisplayCode { get; set; }
    }
}

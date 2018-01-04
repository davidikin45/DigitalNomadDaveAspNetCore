using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public bool OptimisedForMobile { get; set; }
        public string Type { get; set; }
        public string BookingNumber { get; set; }
    }
}

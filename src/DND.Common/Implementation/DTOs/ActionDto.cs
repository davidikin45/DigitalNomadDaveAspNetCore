using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.DTOs
{
    public class BulkActionDto : ActionDto
    {
        public object Id { get; set; }
    }

    public class ActionDto
    {
        public string Action {get; set;}
        public dynamic Args { get; set; }
    }
}

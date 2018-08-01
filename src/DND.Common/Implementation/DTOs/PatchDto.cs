using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.DTOs
{
    public class PatchDto
    {
       public object Id { get; set; }
       public JsonPatchDocument Commands { get; set; }
    }
}

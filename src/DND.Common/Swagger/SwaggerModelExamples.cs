using Microsoft.AspNetCore.JsonPatch;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Swagger
{
    public class SwaggerModelExamples : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (context.SystemType == typeof(Microsoft.AspNetCore.JsonPatch.Operations.Operation))
            {
                model.Example = new { op = "add/replace/remove/copy/move/test", path = "/property", value = "value", from = "/property" };
            };
        }
    }
}

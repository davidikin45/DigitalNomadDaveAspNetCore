using DND.Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Linq;

namespace DND.Common.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void UseDbGeographyModelBinding(this MvcOptions opts)
        {       
            var binderToFind = opts.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
           
            if (binderToFind == null) return;

            var index = opts.ModelBinderProviders.IndexOf(binderToFind);
            opts.ModelBinderProviders.Insert(index, new DbGeographyModelBinderProvider());
        }
    }
}

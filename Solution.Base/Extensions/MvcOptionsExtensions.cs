using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void UseCustomModelBinding(this MvcOptions opts)
        {


         
            var binderToFind = opts.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
           
            if (binderToFind == null) return;

            var index = opts.ModelBinderProviders.IndexOf(binderToFind);
            opts.ModelBinderProviders.Insert(index, new CustomModelBinderProvider());
        }

        public static void UseCustomModelValidation(this MvcOptions opts)
        {

   
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure
{
    public class DbGeographyModelBinder : IModelBinder
    {
      
        private readonly IModelBinder _fallbackBinder;

        public DbGeographyModelBinder(IModelBinder fallbackBinder)
        {
            if (fallbackBinder == null)
                throw new ArgumentNullException(nameof(fallbackBinder));

            _fallbackBinder = fallbackBinder;
        }


        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return _fallbackBinder.BindModelAsync(bindingContext);
            }

            var valueAsString = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(valueAsString))
            {
                return _fallbackBinder.BindModelAsync(bindingContext);
            }

            DbGeography result = null;
          
            string[] latLongStr = valueAsString.Split(',');
            string point = string.Format("POINT ({0} {1})", latLongStr[1], latLongStr[0]);

            //4326 format puts LONGITUDE first then LATITUDE
            result = DbGeography.FromText(point, 4326);

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }

    public class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DbGeography))
            {
                return new DbGeographyModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType));
            }
            return null;
        }
    }
}

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Helpers
{
    public static class BootstrapHelperExtension
    {
        public static BootstrapHelper<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> helper)
        {
            return new BootstrapHelper<TModel>(helper);
        }

        public class BootstrapHelper<TModel>
        {

            private readonly IHtmlHelper<TModel> _htmlHelper;


            public BootstrapHelper(IHtmlHelper<TModel> helper)
            {
                _htmlHelper = helper;
            }

            public IHtmlContent BootstrapLabelFor<TProp>(Expression<Func<TModel, TProp>> property)
            {
                var label = _htmlHelper.LabelFor(property, null, new
                {
                    @class = "col-md-2 form-control-label col-form-label"
                });

                return label;          
            }
        }

        public static IHtmlContent BootstrapLabel(this IHtmlHelper helper, string propertyName)
        {
            var label = helper.Label(propertyName, null, new
            {
                @class = "col-md-2 form-control-label col-form-label"
            });

            return label;
        }
    }
}

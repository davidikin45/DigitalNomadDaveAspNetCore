using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Helpers
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
                return _htmlHelper.LabelFor(property, new
                {
                    @class = "col-md-2 form-control-label col-form-label"
                });
            }

        }

        public static IHtmlContent BootstrapLabel(this IHtmlHelper helper, string propertyName)
        {
            return helper.LabelForModel(propertyName, new
            {
                @class = "col-md-2 form-control-label col-form-label"
            });
        }


    }
}

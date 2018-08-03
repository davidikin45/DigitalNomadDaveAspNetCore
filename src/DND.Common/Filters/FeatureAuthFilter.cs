using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Filters
{
    //Add this to controller to enable and disable features
    //[TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Loan" })]
    public class FeatureAuthFilter : IAuthorizationFilter
    {
        private IFeatureService featureService;
        private string featureName;

        public FeatureAuthFilter(IFeatureService featureService, string featureName)
        {
            this.featureService = featureService;
            this.featureName = featureName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!featureService.IsFeatureActive(featureName))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}

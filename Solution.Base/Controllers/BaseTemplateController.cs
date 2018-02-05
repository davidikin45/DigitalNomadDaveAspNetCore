using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Controllers
{
    public abstract class BaseTemplateController : BaseController
    {


        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/wwwroot/js/app/{0}/templates/{1}", feature, name));
        }


    }
}

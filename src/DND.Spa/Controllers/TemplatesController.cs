using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DND.Spa.Controllers
{
    public class TemplatesController : Controller
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/ClientApp/src/app/{0}/templates/{1}", feature, name));
        }
    }
}
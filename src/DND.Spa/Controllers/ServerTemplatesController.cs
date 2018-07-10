using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DND.Spa.Controllers
{
    public class ServerTemplatesController : Controller
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/ClientApp/src/app/{0}/server-templates/{1}", feature, name));
        }
    }
}
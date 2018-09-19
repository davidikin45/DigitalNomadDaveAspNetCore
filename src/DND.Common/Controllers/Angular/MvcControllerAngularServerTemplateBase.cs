using Microsoft.AspNetCore.Mvc;

namespace DND.Common.Controllers
{
    public abstract class MvcControllerAngularServerTemplateBase : MvcControllerBase
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/wwwroot/js/app/{0}/templates/{1}", feature, name));
        }
    }
}

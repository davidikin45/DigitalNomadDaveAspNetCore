using Microsoft.AspNetCore.Mvc;

namespace DND.Common.Controllers
{
    public abstract class MvcControllerAngular6ServerTemplateBase : MvcControllerBase
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/ClientApp/src/app/{0}/server-templates/{1}", feature, name));
        }
    }
}

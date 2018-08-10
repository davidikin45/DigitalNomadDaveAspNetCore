using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Services
{
    public interface IHtmlHelperGeneratorService
    {
        IHtmlHelper HtmlHelper<TModel>(TModel model);
    }
}

using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


namespace DND.Common.Implementation.Services
{
    public class HtmlHelperGeneratorService : IHtmlHelperGeneratorService
    {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IViewBufferScope _viewBufferScope;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HtmlHelperOptions _htmlHelperOptions;

        public HtmlHelperGeneratorService(IHtmlGenerator htmlGenerator, ICompositeViewEngine compositeViewEngine, IModelMetadataProvider modelMetadataProvider, IViewBufferScope viewBufferScope, IActionContextAccessor actionContextAccessor, ITempDataProvider tempDataProvider, IOptions<MvcViewOptions> options)
        {
            _htmlGenerator = htmlGenerator;
            _compositeViewEngine = compositeViewEngine;
            _modelMetadataProvider = modelMetadataProvider;
            _viewBufferScope = viewBufferScope;
            _actionContextAccessor = actionContextAccessor;
            _tempDataProvider = tempDataProvider;
            _htmlHelperOptions = options.Value.HtmlHelperOptions;
        }

        public IHtmlHelper HtmlHelper<TModel>(TModel model)
        {
            var tempData = new TempDataDictionary(_actionContextAccessor.ActionContext.HttpContext, _tempDataProvider);
            var newViewData = new ViewDataDictionary<TModel>(_modelMetadataProvider, new ModelStateDictionary()) { Model = model };

            var helper = new HtmlHelper(_htmlGenerator, _compositeViewEngine, _modelMetadataProvider, _viewBufferScope, HtmlEncoder.Default, UrlEncoder.Default);
            var viewContext = new ViewContext(_actionContextAccessor.ActionContext,
               new FakeView(),
               newViewData,
               tempData,
               TextWriter.Null,
               _htmlHelperOptions);
            helper.Contextualize(viewContext);
            return helper;
        }

        public IHtmlHelper HtmlHelper(dynamic model)
        {
            throw new System.NotImplementedException();
        }

        private class FakeView : IView
        {
            public string Path => "View";

            public Task RenderAsync(ViewContext context)
            {
                return Task.FromResult(0);
            }
        }
    }
}

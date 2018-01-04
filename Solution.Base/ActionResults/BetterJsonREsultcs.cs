using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Solution.Base.Alerts;
using Solution.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Solution.Base.ActionResults
{
    public class BetterJsonResult : JsonResult
    {
        public int ErrorStatusCode = 400;
        public IList<string> Errors { get; private set; }
        public string Message { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public BetterJsonResult()
            :base(null)
        {
            Errors = new List<string>();
        }

        public BetterJsonResult(string message)
             : base(null)
        {
            Message = message;
            Errors = new List<string>();
        }

        public BetterJsonResult(string message, ModelStateDictionary modelState)
             : base(null)
        {
            Message = message;
            Errors = new List<string>();
            ModelState = modelState;
        }

        public void AddError(string errorMessage)
        {
            Errors.Add(errorMessage);
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            DoUninterestingBaseClassStuff(context);

           await SerializeDataAsync(context.HttpContext.Response);
        }

        private void DoUninterestingBaseClassStuff(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
        }

        protected async virtual Task SerializeDataAsync(HttpResponse response)
        {
            if (Errors.Any())
            {
                Value = BuildWebApiMessageObject();

                response.StatusCode = ErrorStatusCode;
            }

            if (Value == null) return;

            await response.WriteAsync(Value.ToJson());
        }

        public WebApiMessage BuildWebApiMessageObject()
        {
            return WebApiMessage.CreateWebApiMessage(Message, Errors, ModelState);
        }
    }

    public class BetterJsonResult<T> : BetterJsonResult
    {
        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
    }
}

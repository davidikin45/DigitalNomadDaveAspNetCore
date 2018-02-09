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
    public class BetterObjectResult : ObjectResult
    {
        public int ErrorStatusCode = 400;
        public IList<string> Errors { get; private set; }
        public string Message { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public BetterObjectResult()
            :base(null)
        {
            Errors = new List<string>();
        }

        public BetterObjectResult(string message)
             : base(null)
        {
            Message = message;
            Errors = new List<string>();
        }

        public BetterObjectResult(string message, ModelStateDictionary modelState)
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
            if (Errors.Any())
            {
                Value = BuildWebApiMessageObject();

                StatusCode = ErrorStatusCode;
            }

            await base.ExecuteResultAsync(context);
        }

        public WebApiMessage BuildWebApiMessageObject()
        {
            return WebApiMessage.CreateWebApiMessage(Message, Errors, ModelState);
        }
    }

    public class BetterObjectResult<T> : BetterObjectResult
    {
        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
    }
}

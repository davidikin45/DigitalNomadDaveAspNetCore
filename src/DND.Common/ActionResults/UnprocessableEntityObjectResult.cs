using DND.Common.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.ActionResults
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(string message, ModelStateDictionary modelState)
            : base(CreateWebApiMessage(message,modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 422; //Unprocessable Entity 
        }

        public UnprocessableEntityObjectResult(IEnumerable<WebApiMessage> webApiMessages)
           : base(webApiMessages)
        {
            if (webApiMessages == null)
            {
                throw new ArgumentNullException(nameof(webApiMessages));
            }
            StatusCode = 422; //Unprocessable Entity 
        }

        public static WebApiMessage CreateWebApiMessage(string message, ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            var errors = modelState.Values.SelectMany(v => v.Errors);
            var errorList = new List<string>();
            foreach (var error in errors)
            {
                errorList.Add(error.ErrorMessage);
            }

            var response = WebApiMessage.CreateWebApiMessage(message, errorList, modelState);

            return response;
        }
    }
}

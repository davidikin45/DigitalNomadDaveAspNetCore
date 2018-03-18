using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Alerts
{
    [DataContract]
    public class WebApiMessage
    {
        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors, Object id)
        {
            return new WebApiMessage(message, errors.ToArray(), id);
        }

        private WebApiMessage(string message, string[] errors, Object id)
        {
            this.Message = message;
            this.Errors = errors;
            this.Id = id;
        }

        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors)
        {
            return new WebApiMessage(message, errors.ToArray());
        }

        private WebApiMessage(string message, string[] errors)
        {
            this.Message = message;
            this.Errors = errors;
        }

        public static WebApiMessage CreateWebApiMessage(string message, IList<string> errors, ModelStateDictionary modelState)
        {
            return new WebApiMessage(message, errors.ToArray(), modelState);
        }

        private WebApiMessage(string message, string[] errors, ModelStateDictionary modelState)
        {
            this.Message = message;
            this.Errors = errors;

            this.ModelState = new Dictionary<string, List<string>>();

            if (modelState != null)
            {

                foreach (KeyValuePair<string, ModelStateEntry> property in modelState)
                {
                    var propertyMessages = new List<string>();
                    foreach (ModelError modelError in property.Value.Errors)
                    {
                        propertyMessages.Add(modelError.ErrorMessage);
                    }
                    this.ModelState.Add(property.Key, propertyMessages);
                }

            }
        }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Id { get; set; }

        [DataMember]
        public Boolean Success
        {
            get
            {
                return Errors.Count() == 0;
            }
            private set { }
        }

        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public string Error
        {
            get
            {
                return string.Join("\n", Errors);
            }
            private set { }
        }

        [DataMember]
        public string[] Errors { get; private set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, List<string>> ModelState { get; private set; }

    }
}

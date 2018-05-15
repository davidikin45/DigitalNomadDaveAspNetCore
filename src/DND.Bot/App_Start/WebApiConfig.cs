using DND.Bot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;

namespace DND.Bot
{
    public static class WebApiConfig
    {
        public static IntentsList IntentHandlers { get; private set; }

        public static void Register(HttpConfiguration config)
        {
            string credential_path = HostingEnvironment.MapPath(@"~/App_Data/ServiceKey.json");
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            //Register Intents
            IntentHandlers = new IntentsList
            {
                { "AMAZON.StopIntent", (cm) => Handlers.CancelIntent.Process(cm) },
                { "AMAZON.CancelIntent", (cm) => Handlers.CancelIntent.Process(cm) },
                { "AMAZON.HelpIntent", (cm) => Handlers.HelpIntent.Process(cm) },
                { "YesIntent", (cm) => Handlers.YesIntent.Process(cm) },
                { "ReservationsIntent", (cm) => Handlers.ReservationsIntent.Process(cm) },
                { "DefaultWelcomeIntent", (cm) => Handlers.WelcomeIntent.Process(cm) }
            };

            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.DateTimeOffset,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

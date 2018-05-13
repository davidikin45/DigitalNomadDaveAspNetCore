using Alexa.NET.Request;
using Alexa.NET.Response;
using DND.Bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DND.Bot.Controllers
{
    //Amazon
    public class AlexaController : ApiController
    {
        public SkillResponse Post(SkillRequest skillRequest)
        {
            var commonModel = CommonModelMapper.AlexaToCommonModel(skillRequest);
            if (commonModel == null)
                return null;

            if (commonModel.Request.State == null || commonModel.Request.State == "COMPLETED")
                commonModel = IntentRouter.Process(commonModel);

            return CommonModelMapper.CommonModelToAlexa(commonModel);
        }

        public string Get()
        {
            return "Hello Alexa!";
        }
    }
}

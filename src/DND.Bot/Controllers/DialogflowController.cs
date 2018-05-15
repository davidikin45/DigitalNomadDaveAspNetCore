using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DND.Bot.Helpers;
using Google.Cloud.Dialogflow.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DND.Bot.Controllers
{
    //Google
    public class DialogflowController : ApiController
    {
        [HttpPost]
        public async Task<dynamic> Post()
        {
            var json = await Request.Content.ReadAsStringAsync();
          
            WebhookRequest request = CommonModelMapper.JsonToDialogflow(json);

            var commonModel = CommonModelMapper.DialogflowToCommonModel(request);
            if (commonModel == null)
                return null;

            commonModel = IntentRouter.Process(commonModel);

            return CommonModelMapper.CommonModelToDialogflow(commonModel);
        }

        public string Get()
        {
            return "Hello Dialogflow!";
        }
    }
} 

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Google.Cloud.Dialogflow.V2;
using ApiAiSDK;
using DND.Bot.Helpers;

namespace DND.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string clientAccessToken = "5f52e2eae55e4ef58c53dab7431efa74";
        private static AIConfiguration config = new AIConfiguration(clientAccessToken, SupportedLanguage.English);
        private static ApiAi apiAi = new ApiAi(config);

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var aiResponse = apiAi.TextRequest(string.IsNullOrWhiteSpace(activity.Text) ? "hello" : activity.Text);

            await context.PostAsync(aiResponse.Result.Fulfillment.Speech);

            context.Wait(MessageReceivedAsync);
        }
    }
}
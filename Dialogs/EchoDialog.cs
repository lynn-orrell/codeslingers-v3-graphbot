using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using BotAuth;
using BotAuth.Models;

namespace Codeslingers.Bots.v3.GraphBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        private IAuthProvider _authProvider;
        private HttpClient _httpClient;
        protected int count = 1;

        public EchoDialog(IAuthProvider authProvider, HttpClient httpClient)
        {
            _authProvider = authProvider;
            _httpClient = httpClient;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // Use token to call into service
            var json = await _httpClient.GetWithAuthAsync(context.UserData.GetValue<AuthResult>($"{_authProvider.Name}{ContextConstants.AuthResultKey}").AccessToken, "https://graph.microsoft.com/v1.0/me");
            await context.PostAsync($"I'm a simple bot that doesn't do much, but I know your name is {json.Value<string>("displayName")} and your UPN is {json.Value<string>("userPrincipalName")}");

            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}
using BotAuth;
using BotAuth.AADv2;
using BotAuth.Dialogs;
using BotAuth.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Codeslingers.Bots.v3.GraphBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private IAuthProvider _authProvider;
        private IDialogFactory _dialogFactory;

        public RootDialog(IAuthProvider authProvider, IDialogFactory dialogFactory)
        {
            _authProvider = authProvider;
            _dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;

            // Initialize AuthenticationOptions and forward to AuthDialog for token
            AuthenticationOptions options = new AuthenticationOptions()
            {
                Authority = ConfigurationManager.AppSettings["Authority"],
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                Scopes = new string[] { "User.Read" },
                RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"]
            };
            await context.Forward(new AuthDialog(_authProvider, options), AuthDialogComplete, message, CancellationToken.None);
        }

        private async Task AuthDialogComplete(IDialogContext context, IAwaitable<AuthResult> authResult)
        {
            var result = await authResult;

            await context.Forward(_dialogFactory.Create<EchoDialog>(), EchoDialogComplete, context.Activity.AsMessageActivity());
        }

        private async Task EchoDialogComplete(IDialogContext context, IAwaitable<object> result)
        {
            var v = await result;
        }
    }
}
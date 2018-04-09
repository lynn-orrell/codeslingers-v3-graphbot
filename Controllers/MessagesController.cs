using Autofac;
using Codeslingers.Bots.v3.GraphBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Codeslingers.Bots.v3.GraphBot
{
    [BotAuthentication()]
    public class MessagesController : ApiController
    {
        //private readonly ILifetimeScope _scope;

        //public MessagesController(ILifetimeScope scope)
        //{
        //    SetField.NotNull(out this._scope, nameof(_scope), scope);
        //}

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    // register the function to provide the root dialog
                    Func<IDialog<object>> makeRoot = () => scope.Resolve<RootDialog>();
                    scope.Resolve<Func<IDialog<object>>>(TypedParameter.From(makeRoot));

                    // start the dialog process by posting the activity to IPostToBot in the 
                    // same scope as the root dialog
                    var task = scope.Resolve<IPostToBot>();
                    await task.PostAsync(activity, CancellationToken.None);
                }
                //await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
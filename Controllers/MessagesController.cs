using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication(MicrosoftAppIdSettingName = "Codeslingers-Bots-v3-GraphBot-MicrosoftAppId", MicrosoftAppPasswordSettingName = "Codeslingers-Bots-v3-GraphBot-MicrosoftAppPassword")]
    public class MessagesController : ApiController
    {
        static MessagesController()
        {
            Conversation.UpdateContainer(builder => builder.Register(c => ((ClaimsIdentity)HttpContext.Current.User.Identity).GetCredentialsFromClaims())
                                                           .AsSelf()
                                                           .InstancePerLifetimeScope());
        }


        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new EchoDialog());
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
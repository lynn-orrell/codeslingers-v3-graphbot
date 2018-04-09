using Autofac;
using System.Web.Http;
using System.Configuration;
using System.Reflection;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using BotAuth.AADv2;
using Codeslingers.Bots.v3.GraphBot.Dialogs;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Net.Http;

namespace Codeslingers.Bots.v3.GraphBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Bot Storage: This is a great spot to register the private state storage for your bot. 
            // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
            // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure

            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                    // Using Azure Table Storage
                    var store = new TableBotDataStore(ConfigurationManager.AppSettings["BotStateStorage"]); // requires Microsoft.BotBuilder.Azure Nuget package 

                    // To use CosmosDb or InMemory storage instead of the default table storage, uncomment the corresponding line below
                    // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 
                    // var store = new InMemoryDataStore(); // volatile in-memory store

                    builder.Register(c => store)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();

                    builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

                    builder.Register<HttpClient>(c => new HttpClient())
                           .Keyed<HttpClient>(FiberModule.Key_DoNotSerialize)
                           .AsSelf()
                           .SingleInstance();

                    builder.RegisterType<DialogFactory>()
                           .Keyed<IDialogFactory>(FiberModule.Key_DoNotSerialize)
                           .AsImplementedInterfaces()
                           .SingleInstance();

                    builder.Register(c => new MSALAuthProvider()).AsImplementedInterfaces().SingleInstance();

                    builder.RegisterType<RootDialog>();
                    builder.RegisterType<EchoDialog>();

                });
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

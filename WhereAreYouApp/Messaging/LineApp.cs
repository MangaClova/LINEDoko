using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhereAreYouApp.Messaging.Contexts;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging
{
    class LineApp : WebhookApplication
    {
        private LineMessagingClient Client { get; }
        private CloudTable StateStoreTable { get; }
        private IBinder Binder { get; }
        private MessagingChatSettings MessagingChatSettings { get; set; }
        private MessagingSessionData MessagingSessionData { get; set; }

        public LineApp(LineMessagingClient client, CloudTable stateStoreTable, IBinder binder)
        {
            Client = client;
            StateStoreTable = stateStoreTable;
            Binder = binder;
        }

        protected override Task OnFollowAsync(FollowEvent ev) => ExecuteAsync(ev);

        protected override Task OnMessageAsync(MessageEvent ev) => ExecuteAsync(ev);

        private async Task ExecuteAsync(WebhookEvent ev)
        {
            await RestoreStateAsync(ev.Source.UserId);
            if (ev.Type == WebhookEventType.Unfollow)
            {
                MessagingChatSettings.IsLineFrend = false;
                MessagingChatSettings.ChatStatus = ChatStatusType.Init;
                await SaveStateAsync(ev.Source.UserId);
                return;
            }

            if (ev.Type == WebhookEventType.Follow)
            {
                MessagingChatSettings.IsLineFrend = true;
                MessagingChatSettings.ChatStatus = ChatStatusType.Init;
            }

            await ContextFactory.Create(MessagingChatSettings.ChatStatus).ExecuteAsync(
                new ContextState
                {
                    Client = Client,
                    SessionData = MessagingSessionData,
                    Settings = MessagingChatSettings,
                    WebhookEvent = ev,
                    StateStoreTable = StateStoreTable,
                    Binder = Binder,
                });
            await SaveStateAsync(ev.Source.UserId);
        }

        private Task SaveStateAsync(string userId)
        {
            return Task.WhenAll(
                StateStoreTable.ExecuteAsync(TableOperation.InsertOrReplace(MessagingChatSettings)),
                StateStoreTable.ExecuteAsync(TableOperation.InsertOrReplace(MessagingSessionData)));
        }

        private async Task RestoreStateAsync(string userId)
        {
            var r = await Task.WhenAll(
                StateStoreTable.ExecuteAsync(TableOperation.Retrieve<MessagingChatSettings>(nameof(MessagingChatSettings), userId)),
                StateStoreTable.ExecuteAsync(TableOperation.Retrieve<MessagingSessionData>(nameof(MessagingSessionData), userId)));
            if (r[0].HttpStatusCode != 200)
            {
                MessagingChatSettings = new MessagingChatSettings
                {
                    RowKey = userId,
                };
            }
            else
            {
                MessagingChatSettings = (MessagingChatSettings)r[0].Result;
            }

            if (r[1].HttpStatusCode != 200)
            {
                MessagingSessionData = new MessagingSessionData
                {
                    RowKey = userId,
                };
            }
            else
            {
                MessagingSessionData = (MessagingSessionData)r[1].Result;
                if (MessagingSessionData.Timestamp <= DateTimeOffset.UtcNow - TimeSpan.FromMinutes(15))
                {
                    MessagingSessionData = new MessagingSessionData
                    {
                        RowKey = userId,
                    };
                }
            }
        }
    }
}

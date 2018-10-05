using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging
{
    class LineApp : WebhookApplication
    {
        private LineMessagingClient Client { get; }
        private CloudTable LocationLogs { get; }

        public LineApp(LineMessagingClient client, CloudTable locationLogs)
        {
            Client = client;
            LocationLogs = locationLogs;
        }

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            if (ev.Type == WebhookEventType.Message)
            {
                if (ev.Message.Type == EventMessageType.Location)
                {
                    var locationMessage = (LocationEventMessage)ev.Message;
                    var locationLog = new LocationLog
                    {
                        RowKey = ev.Source.UserId,
                        Latitude = locationMessage.Latitude,
                        Longitude = locationMessage.Longitude,
                        Name = locationMessage.Title,
                        Address = locationMessage.Address,
                    };
                    await LocationLogs.ExecuteAsync(TableOperation.InsertOrReplace(locationLog));
                    await Client.ReplyMessageAsync(ev.ReplyToken, "お子さんにお伝えしておきます。");
                    return;
                }
            }

            await Client.ReplyMessageAsync("まだ対応してません。");
        }
    }
}

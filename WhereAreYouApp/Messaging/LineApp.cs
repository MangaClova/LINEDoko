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
                    await Client.ReplyMessageAsync(ev.ReplyToken, "ありがとう！場所の記録を消したいときは「消す」って話しかけてね。");
                    return;
                }

                if (ev.Message.Type == EventMessageType.Text)
                {
                    var textMessage = (TextEventMessage)ev.Message;
                    if (textMessage.Text.Trim() == "消す")
                    {
                        var targetLog = await LocationLogs.ExecuteAsync(TableOperation.Retrieve<LocationLog>(
                            nameof(LocationLog), ev.Source.UserId));
                        if (targetLog.HttpStatusCode == 200)
                        {
                            await LocationLogs.ExecuteAsync(TableOperation.Delete((LocationLog)targetLog.Result));
                        }

                        await Client.ReplyMessageAsync(ev.ReplyToken, new List<ISendMessage>
                        {
                            new TextMessage("場所の記録を削除しました。また登録するときは現在地を送るを押してね。", new QuickReply(
                                new List<QuickReplyButtonObject>
                                {
                                    new QuickReplyButtonObject(new LocationTemplateAction("現在地を送る")),
                                })),
                        });
                        return;
                    }
                }
            }

            await Client.ReplyMessageAsync("まだ対応してません。");
        }
    }
}

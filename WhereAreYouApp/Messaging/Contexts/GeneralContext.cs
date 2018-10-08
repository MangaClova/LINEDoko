using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class GeneralContext : StatefulContextBase<MessageEvent>
    {
        private Dictionary<EventMessageType, Func<ContextState, MessageEvent, Task>> ActionMap { get; }

        public GeneralContext()
        {
            ActionMap = new Dictionary<EventMessageType, Func<ContextState, MessageEvent, Task>>
            {
                [EventMessageType.Location] = ExecuteLocationMessageAsync,
                [EventMessageType.Text] = ExecuteTextMessageAsync,
            };
        }

        private async Task ExecuteTextMessageAsync(ContextState contextState, MessageEvent messageEvent)
        {
            var text = ((TextEventMessage)messageEvent.Message).Text;
            switch (LineMessages.DetectIntent(text))
            {
                case IntentType.ChangeName:
                    await contextState.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineMessages.GetAskYourNameMessage());
                    contextState.Settings.ChatStatus = ChatStatusType.AskingYourName;
                    break;
                case IntentType.ShowHistory:
                    await ShowHistoryAsync(contextState, messageEvent);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown intent: {text}");
            }
        }

        private Task ShowHistoryAsync(ContextState contextState, MessageEvent messageEvent)
        {
            // 履歴機能
            throw new NotImplementedException();
        }

        private async Task ExecuteLocationMessageAsync(ContextState contextState, MessageEvent messageEvent)
        {
            var locationMessage = (LocationEventMessage)messageEvent.Message;
            var locationLog = new LocationLog
            {
                RowKey = messageEvent.Source.UserId,
                Latitude = locationMessage.Latitude,
                Longitude = locationMessage.Longitude,
                Name = locationMessage.Title,
                Address = locationMessage.Address,
            };

            await contextState.StateStoreTable.ExecuteAsync(TableOperation.InsertOrReplace(locationLog));
            await contextState.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineMessages.GetConfirmMessage(LineMessages.AskCommentMessage));
            SetNextCallMethod(nameof(AskingInputCommentAsync));
        }

        private async Task AskingInputCommentAsync(ContextState contextState, MessageEvent ev)
        {
            bool isValidMessage(out TextEventMessage textEventMessage)
            {
                if (ev.Message.Type != EventMessageType.Text)
                {
                    textEventMessage = null;
                    return false;
                }

                textEventMessage = (TextEventMessage)ev.Message;
                return LineMessages.IsYesOrNo(textEventMessage.Text);
            }

            if (!isValidMessage(out var text))
            {
                await contextState.Client.ReplyMessageAsync(ev.ReplyToken, LineMessages.GetConfirmMessage(LineMessages.AskCommentMessage));
                SetNextCallMethod(nameof(AskingInputCommentAsync));
                return;
            }

            if (LineMessages.IsYes(text.Text))
            {
                await contextState.Client.ReplyMessageAsync(ev.ReplyToken, LineMessages.GetInputCommentMessage());
                SetNextCallMethod(nameof(InputCommentAsync));
                return;
            }
            else
            {
                await FinishInputAsync(contextState, ev);
            }
        }

        private async Task FinishInputAsync(ContextState contextState, MessageEvent ev, LocationLog locationLog = null)
        {
            locationLog = locationLog ?? await LocationLog.GetLocationLogByUserIdAsync(contextState.StateStoreTable, ev.Source.UserId);
            if (locationLog == null)
            {
                await ReplyUnknownMessageAsync(contextState, ev);
                return;
            }

            await contextState.Client.ReplyMessageAsync(ev.ReplyToken, LineMessages.GetFinishInputMessage(contextState.Settings, locationLog));
            SetNextCallMethod(nameof(InitializeAsync));
        }

        private async Task InputCommentAsync(ContextState contextState, MessageEvent ev)
        {
            if (ev.Message.Type == EventMessageType.Text)
            {
                var locationLog = await LocationLog.GetLocationLogByUserIdAsync(contextState.StateStoreTable, ev.Source.UserId);
                if (locationLog == null)
                {
                    await ReplyUnknownMessageAsync(contextState, ev);
                    return;
                }

                locationLog.Comment = ((TextEventMessage)ev.Message).Text;
                await LocationLog.InsertOrReplaceAsync(contextState.StateStoreTable, locationLog);
                await FinishInputAsync(contextState, ev, locationLog);
                SetNextCallMethod(nameof(InitializeAsync));
            }
            else if (ev.Message.Type == EventMessageType.Audio)
            {
                var locationLog = await LocationLog.GetLocationLogByUserIdAsync(contextState.StateStoreTable, ev.Source.UserId);
                if (locationLog == null)
                {
                    await ReplyUnknownMessageAsync(contextState, ev);
                    return;
                }

                using (var audio = await contextState.Client.GetContentStreamAsync(ev.Message.Id))
                {
                    var blob = await contextState.Binder.BindAsync<CloudBlockBlob>(new BlobAttribute($"files/{ev.Source.UserId}/{ev.Message.Id}{GetFileExtension(audio.ContentHeaders.ContentType.MediaType)}", FileAccess.Write));
                    await blob.UploadFromStreamAsync(audio);
                    locationLog.AudioCommentUrl = blob.Uri.ToString();
                }

                await LocationLog.InsertOrReplaceAsync(contextState.StateStoreTable, locationLog);
                await FinishInputAsync(contextState, ev, locationLog);
                SetNextCallMethod(nameof(InitializeAsync));
            }
            else
            {
                await ReplyUnknownMessageAsync(contextState, ev);
            }
        }

        protected override Task InitializeAsync(ContextState contextState, MessageEvent ev)
        {
            return HandleMessageEventAsync(contextState, ev);
        }

        private async Task HandleMessageEventAsync(ContextState contextState, MessageEvent ev)
        {
            if (ActionMap.TryGetValue(ev.Message.Type, out var action))
            {
                await action(contextState, ev);
                return;
            }

            await ReplyUnknownMessageAsync(contextState, ev);
        }

        private async Task ReplyUnknownMessageAsync(ContextState contextState, MessageEvent messageEvent)
        {
            await contextState.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineMessages.GetReplyMessageForUnknownMessageType());
            SetNextCallMethod(nameof(InitializeAsync));
        }

        private string GetFileExtension(string mediaType)
        {
            switch (mediaType)
            {
                case "image/jpeg":
                    return ".jpeg";
                case "audio/x-m4a":
                    return ".m4a";
                case "video/mp4":
                    return ".mp4";
                default:
                    return "";
            }
        }
    }
}

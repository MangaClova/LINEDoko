using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class ManualInputYourNameContext : ContextBase<MessageEvent>
    {
        protected override async Task ExecuteImplAsync(MessagingChatSettings settings, MessagingSessionData sessionData, LineMessagingClient client, MessageEvent messageEvent)
        {
            var textMessage = (TextEventMessage)messageEvent.Message;
            settings.YourName = textMessage.Text.Trim();
            await client.ReplyMessageAsync(messageEvent.ReplyToken, LineReplyMessages.GetFinishGreetingMessage(settings.YourName));
            settings.ChatStatus = ChatStatusType.General;
        }
    }
}

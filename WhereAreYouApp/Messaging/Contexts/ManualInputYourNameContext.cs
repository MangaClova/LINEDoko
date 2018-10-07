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
        protected override async Task ExecuteImplAsync(ContextState context, MessageEvent messageEvent)
        {
            var textMessage = (TextEventMessage)messageEvent.Message;
            context.Settings.YourName = textMessage.Text.Trim();
            await context.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineReplyMessages.GetFinishGreetingMessage(context.Settings.YourName));
            context.Settings.ChatStatus = ChatStatusType.General;
        }
    }
}

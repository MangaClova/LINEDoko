using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Line.Messaging;
using Line.Messaging.Webhooks;
using WhereAreYouApp.Models;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class AskingYourNameContext : ContextBase<MessageEvent>
    {
        protected override async Task ExecuteImplAsync(
            ContextState contextState,
            MessageEvent messageEvent)
        {
            var m = (TextEventMessage)messageEvent.Message;
            if (m.Text == "その他")
            {
                await contextState.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineReplyMessages.GetManualInputMessage());
                contextState.Settings.ChatStatus = ChatStatusType.ManualInputYourName;
                return;
            }

            contextState.Settings.YourName = m.Text.Trim();
            await contextState.Client.ReplyMessageAsync(messageEvent.ReplyToken, LineReplyMessages.GetFinishGreetingMessage(contextState.Settings.YourName));
            contextState.Settings.ChatStatus = ChatStatusType.General;
        }
    }
}
